using BonAirCampaigns.Api.Dtos;
using BonAirCampaigns.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BonAirCampaigns.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<MeResponse>> Me(CancellationToken ct)
    {
        var user = await currentUser.GetOrCreateUserAsync(ct);
        var org = await currentUser.GetOrganizationAsync(ct);

        return new MeResponse(
            user.Id,
            user.Email,
            user.FullName,
            user.Role,
            user.IsPlatformAdmin,
            new OrganizationDto(
                org.Id, org.Name, org.Slug, org.Plan,
                org.CampaignLimit, org.CampaignsUsedThisMonth,
                org.SubscriptionStatus, org.WhiteLabelEnabled));
    }
}
