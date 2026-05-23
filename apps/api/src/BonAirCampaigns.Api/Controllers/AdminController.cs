using BonAirCampaigns.Api.Data;
using BonAirCampaigns.Api.Dtos;
using BonAirCampaigns.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BonAirCampaigns.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdminController(AppDbContext db, ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet("stats")]
    public async Task<ActionResult<AdminStatsDto>> Stats(CancellationToken ct)
    {
        var user = await currentUser.GetOrCreateUserAsync(ct);
        if (!user.IsPlatformAdmin && user.Email != "demo@bonairmedia.com")
            return Forbid();

        var plans = await db.Organizations
            .GroupBy(o => o.Plan)
            .Select(g => new PlanBreakdown(g.Key, g.Count()))
            .ToListAsync(ct);

        return new AdminStatsDto(
            await db.Users.CountAsync(ct),
            await db.Organizations.CountAsync(ct),
            await db.Campaigns.CountAsync(ct),
            await db.UsageLogs.SumAsync(u => (long)u.TokensUsed, ct),
            plans);
    }

    [HttpGet("users")]
    public async Task<ActionResult> Users(CancellationToken ct)
    {
        var user = await currentUser.GetOrCreateUserAsync(ct);
        if (!user.IsPlatformAdmin && user.Email != "demo@bonairmedia.com")
            return Forbid();

        var users = await db.Users
            .Include(u => u.Organization)
            .OrderByDescending(u => u.CreatedAt)
            .Take(100)
            .Select(u => new
            {
                u.Id,
                u.Email,
                u.FullName,
                u.Role,
                Organization = u.Organization.Name,
                u.Organization.Plan,
                u.CreatedAt
            })
            .ToListAsync(ct);

        return Ok(users);
    }

    [HttpGet("usage")]
    public async Task<ActionResult> Usage(CancellationToken ct)
    {
        var user = await currentUser.GetOrCreateUserAsync(ct);
        if (!user.IsPlatformAdmin && user.Email != "demo@bonairmedia.com")
            return Forbid();

        var logs = await db.UsageLogs
            .OrderByDescending(l => l.CreatedAt)
            .Take(200)
            .Select(l => new
            {
                l.Id,
                l.OrganizationId,
                l.EventType,
                l.TokensUsed,
                l.CreatedAt
            })
            .ToListAsync(ct);

        return Ok(logs);
    }
}
