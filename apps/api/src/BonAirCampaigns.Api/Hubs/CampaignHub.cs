using System.Security.Claims;
using BonAirCampaigns.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BonAirCampaigns.Api.Hubs;

[Authorize]
public class CampaignHub(AppDbContext db) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var organizationId = await GetUserOrganizationIdAsync();
        if (organizationId is not null)
            await Groups.AddToGroupAsync(Context.ConnectionId, organizationId.Value.ToString());

        await base.OnConnectedAsync();
    }

    public async Task JoinOrganization(string organizationId)
    {
        if (!Guid.TryParse(organizationId, out var requestedOrganizationId))
            throw new HubException("Invalid organization id.");

        var userOrganizationId = await GetUserOrganizationIdAsync();
        if (userOrganizationId is null || userOrganizationId != requestedOrganizationId)
            throw new HubException("Not authorized to join organization.");

        await Groups.AddToGroupAsync(Context.ConnectionId, organizationId);
    }

    private async Task<Guid?> GetUserOrganizationIdAsync()
    {
        var supabaseUserId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? Context.User?.FindFirstValue("sub");

        if (string.IsNullOrWhiteSpace(supabaseUserId))
            return null;

        return await db.Users
            .Where(u => u.SupabaseUserId == supabaseUserId)
            .Select(u => (Guid?)u.OrganizationId)
            .FirstOrDefaultAsync(Context.ConnectionAborted);
    }
}
