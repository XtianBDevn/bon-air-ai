using System.Text.Json;
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
public class DashboardController(AppDbContext db, ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get(CancellationToken ct)
    {
        var user = await currentUser.GetOrCreateUserAsync(ct);
        var orgId = user.OrganizationId;

        var campaigns = await db.Campaigns.Where(c => c.OrganizationId == orgId).ToListAsync(ct);
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var tokensThisMonth = await db.UsageLogs
            .Where(u => u.OrganizationId == orgId && u.CreatedAt >= monthStart)
            .SumAsync(u => (int?)u.TokensUsed, ct) ?? 0;

        var recentCampaigns = campaigns
            .OrderByDescending(c => c.CreatedAt)
            .Take(6)
            .Select(c => new CampaignDto(
                c.Id, c.Name, c.Status, c.BusinessType, c.Offer, c.TargetAudience,
                c.Location, c.Tone, c.Budget,
                JsonSerializer.Deserialize<List<string>>(c.ChannelsJson) ?? new(),
                c.Headline, c.Cta, c.TokensUsed, c.ProgressPercent,
                c.CreatedAt, c.CompletedAt,
                string.IsNullOrEmpty(c.GeneratedJson) ? null : JsonSerializer.Deserialize<object>(c.GeneratedJson)))
            .ToList();

        var recentAssets = await db.CampaignAssets
            .Where(a => a.Campaign.OrganizationId == orgId)
            .OrderByDescending(a => a.CreatedAt)
            .Take(8)
            .Select(a => new CampaignAssetDto(a.Id, a.AssetType, a.Title, a.Content, a.CreatedAt))
            .ToListAsync(ct);

        var usageChart = await db.UsageLogs
            .Where(u => u.OrganizationId == orgId && u.CreatedAt >= DateTime.UtcNow.AddDays(-14))
            .GroupBy(u => u.CreatedAt.Date)
            .Select(g => new UsageChartPoint(g.Key.ToString("yyyy-MM-dd"), g.Sum(x => x.TokensUsed)))
            .OrderBy(x => x.Date)
            .ToListAsync(ct);

        var org = await currentUser.GetOrganizationAsync(ct);

        return new DashboardDto(
            campaigns.Count,
            campaigns.Count(c => c.Status is "generating" or "queued"),
            campaigns.Count(c => c.Status == "completed"),
            tokensThisMonth,
            Math.Max(0, org.CampaignLimit - org.CampaignsUsedThisMonth),
            org.Plan,
            recentCampaigns,
            recentAssets,
            usageChart);
    }
}
