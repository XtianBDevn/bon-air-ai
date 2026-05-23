namespace BonAirCampaigns.Api.Entities;

public class Subscription
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public string StripeSubscriptionId { get; set; } = string.Empty;
    public string StripePriceId { get; set; } = string.Empty;
    public string Plan { get; set; } = "starter";
    public string Status { get; set; } = "active";
    public DateTime? CurrentPeriodEnd { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
