using BonAirCampaigns.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BonAirCampaigns.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillingController(IStripeBillingService billing, ICurrentUserService currentUser) : ControllerBase
{
    [HttpPost("checkout")]
    [Authorize]
    public async Task<ActionResult<object>> Checkout([FromBody] Dtos.CheckoutRequest request, CancellationToken ct)
    {
        var org = await currentUser.GetOrganizationAsync(ct);
        var url = await billing.CreateCheckoutSessionAsync(org.Id, request.Plan, request.SuccessUrl, request.CancelUrl, ct);
        return Ok(new { url });
    }

    [HttpPost("webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> Webhook(CancellationToken ct)
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync(ct);
        var signature = Request.Headers["Stripe-Signature"].ToString();
        await billing.HandleWebhookAsync(json, signature, ct);
        return Ok();
    }

    [HttpGet("plans")]
    [AllowAnonymous]
    public ActionResult<object> Plans() => Ok(new[]
    {
        new { id = "starter", name = "Starter", price = 49, campaigns = 10, features = new[] { "10 campaigns/mo", "All channels", "PDF export" } },
        new { id = "growth", name = "Growth", price = 149, campaigns = 100, features = new[] { "100 campaigns/mo", "Priority generation", "Team seats" } },
        new { id = "agency", name = "Agency", price = 399, campaigns = -1, features = new[] { "Unlimited campaigns", "White label", "Custom domain" } }
    });
}
