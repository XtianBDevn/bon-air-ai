using System.Text.Json;
using BonAirCampaigns.Api.Agents;
using BonAirCampaigns.Api.Data;
using BonAirCampaigns.Api.Entities;
using BonAirCampaigns.Api.Hubs;
using BonAirCampaigns.Api.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BonAirCampaigns.Api.Services;

public interface ICampaignGenerationService
{
    Task GenerateAsync(Guid campaignId, CancellationToken ct = default);
}

public class CampaignGenerationService(
    AppDbContext db,
    ICampaignOrchestrator orchestrator,
    IHubContext<CampaignHub> hub) : ICampaignGenerationService
{
    public async Task GenerateAsync(Guid campaignId, CancellationToken ct = default)
    {
        var campaign = await db.Campaigns
            .Include(c => c.Organization)
            .FirstOrDefaultAsync(c => c.Id == campaignId, ct)
            ?? throw new InvalidOperationException("Campaign not found");

        campaign.Status = "generating";
        campaign.ProgressPercent = 5;
        await db.SaveChangesAsync(ct);
        await NotifyAsync(campaign);

        var org = campaign.Organization;
        if (org.CampaignsUsedThisMonth >= org.CampaignLimit && org.Plan != "agency")
            throw new InvalidOperationException("Campaign limit reached for this billing period.");

        try
        {
            var channels = JsonSerializer.Deserialize<List<string>>(campaign.ChannelsJson) ?? new();
            var input = new CampaignInput
            {
                BusinessType = campaign.BusinessType,
                Offer = campaign.Offer,
                TargetAudience = campaign.TargetAudience,
                Location = campaign.Location,
                Tone = campaign.Tone,
                Budget = campaign.Budget,
                Channels = channels
            };

            campaign.ProgressPercent = 20;
            await db.SaveChangesAsync(ct);
            await NotifyAsync(campaign);

            var result = await orchestrator.RunAsync(input, ct);
            var tokens = orchestrator.LastTokensUsed;

            campaign.GeneratedJson = JsonSerializer.Serialize(result);
            campaign.Headline = result.Headline;
            campaign.Cta = result.Cta;
            campaign.TokensUsed = tokens;
            campaign.ProgressPercent = 90;
            campaign.Status = "completed";
            campaign.CompletedAt = DateTime.UtcNow;
            org.CampaignsUsedThisMonth += 1;

            await PersistAssetsAsync(campaign, result, ct);

            db.UsageLogs.Add(new UsageLog
            {
                Id = Guid.NewGuid(),
                OrganizationId = org.Id,
                UserId = campaign.CreatedByUserId,
                CampaignId = campaign.Id,
                EventType = "campaign_generated",
                TokensUsed = tokens
            });

            campaign.ProgressPercent = 100;
            await db.SaveChangesAsync(ct);
            await NotifyAsync(campaign);
        }
        catch
        {
            campaign.Status = "failed";
            campaign.ProgressPercent = 0;
            await db.SaveChangesAsync(ct);
            await NotifyAsync(campaign);
            throw;
        }
    }

    private async Task PersistAssetsAsync(Campaign campaign, CampaignGenerationResult result, CancellationToken ct)
    {
        var assets = new List<CampaignAsset>();

        foreach (var (ad, i) in result.FacebookAds.Select((a, i) => (a, i)))
        {
            assets.Add(new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = campaign.Id,
                AssetType = "facebook_ad",
                Title = $"Facebook Ad {i + 1}",
                Content = $"{ad.Headline}\n\n{ad.PrimaryText}\n\nCTA: {ad.Cta}"
            });
        }

        foreach (var (ad, i) in result.GoogleAds.Select((a, i) => (a, i)))
        {
            assets.Add(new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = campaign.Id,
                AssetType = "google_ad",
                Title = $"Google Ad {i + 1}",
                Content = $"{ad.Headline1} | {ad.Headline2}\n{ad.Description}"
            });
        }

        foreach (var (email, i) in result.Emails.Select((e, i) => (e, i)))
        {
            assets.Add(new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = campaign.Id,
                AssetType = "email",
                Title = $"Email {i + 1}: {email.Subject}",
                Content = $"Subject: {email.Subject}\n\n{email.Body}"
            });
        }

        assets.Add(new CampaignAsset
        {
            Id = Guid.NewGuid(),
            CampaignId = campaign.Id,
            AssetType = "seo",
            Title = "SEO Package",
            Content = JsonSerializer.Serialize(result.Seo, new JsonSerializerOptions { WriteIndented = true })
        });

        assets.Add(new CampaignAsset
        {
            Id = Guid.NewGuid(),
            CampaignId = campaign.Id,
            AssetType = "landing_page",
            Title = "Landing Page Copy",
            Content = JsonSerializer.Serialize(result.LandingPage, new JsonSerializerOptions { WriteIndented = true })
        });

        if (!string.IsNullOrEmpty(result.RedditLaunch))
        {
            assets.Add(new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = campaign.Id,
                AssetType = "reddit",
                Title = "Reddit Launch",
                Content = result.RedditLaunch
            });
        }

        db.CampaignAssets.AddRange(assets);
        await db.SaveChangesAsync(ct);
    }

    private Task NotifyAsync(Campaign campaign) =>
        hub.Clients.Group(campaign.OrganizationId.ToString()).SendAsync("CampaignUpdated", new
        {
            campaign.Id,
            campaign.Status,
            campaign.ProgressPercent,
            campaign.TokensUsed,
            campaign.Headline
        });
}
