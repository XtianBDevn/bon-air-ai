using BonAirCampaigns.Api.Data;
using BonAirCampaigns.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace BonAirCampaigns.Api.Services;

public interface IStripeBillingService
{
    Task<string> CreateCheckoutSessionAsync(Guid organizationId, string plan, string successUrl, string cancelUrl, CancellationToken ct = default);
    Task HandleWebhookAsync(string json, string signature, CancellationToken ct = default);
}

public class StripeBillingService(AppDbContext db, IConfiguration config) : IStripeBillingService
{
    private static readonly Dictionary<string, (string Plan, int Limit, bool WhiteLabel)> PlanMap = new()
    {
        ["starter"] = ("starter", 10, false),
        ["growth"] = ("growth", 100, false),
        ["agency"] = ("agency", int.MaxValue, true)
    };

    public async Task<string> CreateCheckoutSessionAsync(Guid organizationId, string plan, string successUrl, string cancelUrl, CancellationToken ct = default)
    {
        StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
        var org = await db.Organizations.FirstAsync(o => o.Id == organizationId, ct);

        if (string.IsNullOrEmpty(org.StripeCustomerId))
        {
            var customerService = new CustomerService();
            var customer = await customerService.CreateAsync(new CustomerCreateOptions
            {
                Email = (await db.Users.FirstAsync(u => u.OrganizationId == org.Id, ct)).Email,
                Name = org.Name,
                Metadata = new Dictionary<string, string> { ["organization_id"] = org.Id.ToString() }
            }, cancellationToken: ct);
            org.StripeCustomerId = customer.Id;
            await db.SaveChangesAsync(ct);
        }

        var priceId = plan switch
        {
            "growth" => config["Stripe:PriceGrowth"] ?? "price_growth",
            "agency" => config["Stripe:PriceAgency"] ?? "price_agency",
            _ => config["Stripe:PriceStarter"] ?? "price_starter"
        };

        var sessionService = new SessionService();
        var session = await sessionService.CreateAsync(new SessionCreateOptions
        {
            Customer = org.StripeCustomerId,
            Mode = "subscription",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            LineItems = [new SessionLineItemOptions { Price = priceId, Quantity = 1 }],
            Metadata = new Dictionary<string, string>
            {
                ["organization_id"] = org.Id.ToString(),
                ["plan"] = plan
            }
        }, cancellationToken: ct);

        return session.Url;
    }

    public async Task HandleWebhookAsync(string json, string signature, CancellationToken ct = default)
    {
        StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
        var webhookSecret = config["Stripe:WebhookSecret"];
        Event stripeEvent;

        if (!string.IsNullOrEmpty(webhookSecret))
        {
            stripeEvent = EventUtility.ConstructEvent(json, signature, webhookSecret);
        }
        else
        {
            stripeEvent = EventUtility.ParseEvent(json);
        }

        if (stripeEvent.Type == "checkout.session.completed" && stripeEvent.Data.Object is Session session)
        {
            var orgId = Guid.Parse(session.Metadata["organization_id"]);
            var plan = session.Metadata.GetValueOrDefault("plan", "starter");
            var org = await db.Organizations.FirstAsync(o => o.Id == orgId, ct);
            ApplyPlan(org, plan);
            org.StripeSubscriptionId = session.SubscriptionId;
            org.SubscriptionStatus = "active";

            db.Subscriptions.Add(new Subscription
            {
                Id = Guid.NewGuid(),
                OrganizationId = orgId,
                StripeSubscriptionId = session.SubscriptionId ?? "",
                StripePriceId = session.Mode,
                Plan = plan,
                Status = "active"
            });
            await db.SaveChangesAsync(ct);
        }
        else if (stripeEvent.Type == "customer.subscription.updated" && stripeEvent.Data.Object is Stripe.Subscription sub)
        {
            var org = await db.Organizations.FirstOrDefaultAsync(o => o.StripeSubscriptionId == sub.Id, ct);
            if (org != null)
            {
                org.SubscriptionStatus = sub.Status;
                await db.SaveChangesAsync(ct);
            }
        }
    }

    private static void ApplyPlan(Organization org, string plan)
    {
        if (!PlanMap.TryGetValue(plan, out var info)) return;
        org.Plan = info.Plan;
        org.CampaignLimit = info.Limit;
        org.WhiteLabelEnabled = info.WhiteLabel;
    }
}
