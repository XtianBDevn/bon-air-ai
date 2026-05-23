using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BonAirCampaigns.Api.Hubs;

[Authorize]
public class CampaignHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var orgId = Context.User?.FindFirst("org_id")?.Value;
        if (!string.IsNullOrEmpty(orgId))
            await Groups.AddToGroupAsync(Context.ConnectionId, orgId);
        await base.OnConnectedAsync();
    }

    public Task JoinOrganization(string organizationId) =>
        Groups.AddToGroupAsync(Context.ConnectionId, organizationId);
}
