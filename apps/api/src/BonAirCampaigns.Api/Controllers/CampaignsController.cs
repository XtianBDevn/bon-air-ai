using System.Text.Json;
using BonAirCampaigns.Api.Data;
using BonAirCampaigns.Api.Dtos;
using BonAirCampaigns.Api.Entities;
using BonAirCampaigns.Api.Jobs;
using BonAirCampaigns.Api.Services;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BonAirCampaigns.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CampaignsController(
    AppDbContext db,
    ICurrentUserService currentUser,
    IBackgroundJobClient jobs) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CampaignDto>>> List(CancellationToken ct)
    {
        var user = await currentUser.GetOrCreateUserAsync(ct);
        var campaigns = await db.Campaigns
            .Where(c => c.OrganizationId == user.OrganizationId)
            .OrderByDescending(c => c.CreatedAt)
            .Take(50)
            .ToListAsync(ct);
        return campaigns.Select(Map).ToList();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CampaignDto>> Get(Guid id, CancellationToken ct)
    {
        var user = await currentUser.GetOrCreateUserAsync(ct);
        var campaign = await db.Campaigns.FirstOrDefaultAsync(c => c.Id == id && c.OrganizationId == user.OrganizationId, ct);
        if (campaign == null) return NotFound();
        return Map(campaign);
    }

    [HttpGet("{id:guid}/assets")]
    public async Task<ActionResult<List<CampaignAssetDto>>> Assets(Guid id, CancellationToken ct)
    {
        var user = await currentUser.GetOrCreateUserAsync(ct);
        var exists = await db.Campaigns.AnyAsync(c => c.Id == id && c.OrganizationId == user.OrganizationId, ct);
        if (!exists) return NotFound();
        var assets = await db.CampaignAssets.Where(a => a.CampaignId == id).OrderBy(a => a.CreatedAt).ToListAsync(ct);
        return assets.Select(a => new CampaignAssetDto(a.Id, a.AssetType, a.Title, a.Content, a.CreatedAt)).ToList();
    }

    [HttpPost]
    public async Task<ActionResult<CampaignDto>> Create([FromBody] CreateCampaignRequest request, CancellationToken ct)
    {
        var user = await currentUser.GetOrCreateUserAsync(ct);
        var org = await currentUser.GetOrganizationAsync(ct);

        if (org.CampaignsUsedThisMonth >= org.CampaignLimit && org.Plan != "agency")
            return BadRequest(new { error = "Campaign limit reached. Upgrade your plan." });

        var campaign = new Campaign
        {
            Id = Guid.NewGuid(),
            OrganizationId = org.Id,
            CreatedByUserId = user.Id,
            Name = request.Name,
            BusinessType = request.BusinessType,
            Offer = request.Offer,
            TargetAudience = request.TargetAudience,
            Location = request.Location,
            Tone = request.Tone,
            Budget = request.Budget,
            ChannelsJson = JsonSerializer.Serialize(request.Channels),
            Status = "queued"
        };

        db.Campaigns.Add(campaign);
        await db.SaveChangesAsync(ct);

        jobs.Enqueue<CampaignGenerationJob>(j => j.Execute(campaign.Id));

        return CreatedAtAction(nameof(Get), new { id = campaign.Id }, Map(campaign));
    }

    [HttpPost("{id:guid}/regenerate")]
    public async Task<ActionResult> Regenerate(Guid id, CancellationToken ct)
    {
        var user = await currentUser.GetOrCreateUserAsync(ct);
        var campaign = await db.Campaigns.FirstOrDefaultAsync(c => c.Id == id && c.OrganizationId == user.OrganizationId, ct);
        if (campaign == null) return NotFound();

        var oldAssets = await db.CampaignAssets.Where(a => a.CampaignId == id).ToListAsync(ct);
        db.CampaignAssets.RemoveRange(oldAssets);
        campaign.Status = "queued";
        campaign.ProgressPercent = 0;
        await db.SaveChangesAsync(ct);

        jobs.Enqueue<CampaignGenerationJob>(j => j.Execute(campaign.Id));
        return Accepted();
    }

    private static CampaignDto Map(Campaign c)
    {
        object? generated = null;
        if (!string.IsNullOrEmpty(c.GeneratedJson))
            generated = JsonSerializer.Deserialize<object>(c.GeneratedJson);

        var channels = JsonSerializer.Deserialize<List<string>>(c.ChannelsJson) ?? new();

        return new CampaignDto(
            c.Id, c.Name, c.Status, c.BusinessType, c.Offer, c.TargetAudience,
            c.Location, c.Tone, c.Budget, channels, c.Headline, c.Cta,
            c.TokensUsed, c.ProgressPercent, c.CreatedAt, c.CompletedAt, generated);
    }
}
