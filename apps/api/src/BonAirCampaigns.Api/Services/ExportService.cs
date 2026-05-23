using System.Text;
using System.Text.Json;
using BonAirCampaigns.Api.Data;
using BonAirCampaigns.Api.Entities;
using BonAirCampaigns.Api.Models;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BonAirCampaigns.Api.Services;

public interface IExportService
{
    Task<ExportRecord> ExportAsync(Guid campaignId, string format, Guid organizationId, CancellationToken ct = default);
}

public class ExportService(AppDbContext db, IWebHostEnvironment env) : IExportService
{
    static ExportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<ExportRecord> ExportAsync(Guid campaignId, string format, Guid organizationId, CancellationToken ct = default)
    {
        var campaign = await db.Campaigns
            .Include(c => c.Assets)
            .FirstOrDefaultAsync(c => c.Id == campaignId && c.OrganizationId == organizationId, ct)
            ?? throw new InvalidOperationException("Campaign not found");

        var record = new ExportRecord
        {
            Id = Guid.NewGuid(),
            CampaignId = campaignId,
            OrganizationId = organizationId,
            Format = format.ToLowerInvariant(),
            Status = "processing"
        };
        db.Exports.Add(record);
        await db.SaveChangesAsync(ct);

        var exportsDir = Path.Combine(env.ContentRootPath, "exports");
        Directory.CreateDirectory(exportsDir);
        var fileName = $"{campaign.Id}-{format}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        string filePath;
        string contentType;

        switch (format.ToLowerInvariant())
        {
            case "markdown":
                filePath = Path.Combine(exportsDir, fileName + ".md");
                await File.WriteAllTextAsync(filePath, BuildMarkdown(campaign), ct);
                contentType = "text/markdown";
                break;
            case "html":
                filePath = Path.Combine(exportsDir, fileName + ".html");
                await File.WriteAllTextAsync(filePath, BuildHtml(campaign), ct);
                contentType = "text/html";
                break;
            default:
                filePath = Path.Combine(exportsDir, fileName + ".pdf");
                Document.Create(container => ComposePdf(container, campaign)).GeneratePdf(filePath);
                contentType = "application/pdf";
                break;
        }

        record.FileUrl = $"/api/exports/{record.Id}/download";
        record.Status = "completed";
        await db.SaveChangesAsync(ct);
        return record;
    }

    private static void ComposePdf(IDocumentContainer container, Campaign campaign)
    {
        container.Page(page =>
        {
            page.Margin(40);
            page.Header().Text(campaign.Name).Bold().FontSize(20);
            page.Content().Column(col =>
            {
                col.Item().Text($"Status: {campaign.Status}").FontSize(10);
                if (!string.IsNullOrEmpty(campaign.Headline))
                    col.Item().PaddingTop(10).Text(campaign.Headline).Bold().FontSize(16);
                if (!string.IsNullOrEmpty(campaign.Cta))
                    col.Item().Text($"CTA: {campaign.Cta}");

                foreach (var asset in campaign.Assets)
                {
                    col.Item().PaddingTop(15).Text(asset.Title).Bold();
                    col.Item().Text(asset.Content).FontSize(10);
                }
            });
            page.Footer().AlignCenter().Text($"Bon Air Media Campaigns — {DateTime.UtcNow:yyyy-MM-dd}");
        });
    }

    private static string BuildMarkdown(Campaign campaign)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# {campaign.Name}");
        sb.AppendLine();
        sb.AppendLine($"- **Business:** {campaign.BusinessType}");
        sb.AppendLine($"- **Offer:** {campaign.Offer}");
        sb.AppendLine($"- **Audience:** {campaign.TargetAudience}");
        sb.AppendLine($"- **Location:** {campaign.Location}");
        sb.AppendLine();
        if (!string.IsNullOrEmpty(campaign.Headline))
        {
            sb.AppendLine($"## Headline\n{campaign.Headline}\n");
            sb.AppendLine($"**CTA:** {campaign.Cta}\n");
        }
        foreach (var asset in campaign.Assets)
        {
            sb.AppendLine($"## {asset.Title}\n");
            sb.AppendLine(asset.Content);
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private static string BuildHtml(Campaign campaign)
    {
        var assets = string.Join("", campaign.Assets.Select(a =>
            "<section><h2>" + System.Net.WebUtility.HtmlEncode(a.Title) +
            "</h2><pre>" + System.Net.WebUtility.HtmlEncode(a.Content) + "</pre></section>"));

        var name = System.Net.WebUtility.HtmlEncode(campaign.Name);
        var headline = System.Net.WebUtility.HtmlEncode(campaign.Headline ?? "");
        var cta = System.Net.WebUtility.HtmlEncode(campaign.Cta ?? "");

        return "<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\"/>" +
            "<title>" + name + "</title>" +
            "<style>body{font-family:Inter,system-ui,sans-serif;max-width:800px;margin:2rem auto;padding:0 1rem;color:#111}" +
            "h1{font-size:2rem}section{margin:2rem 0;padding:1rem;border:1px solid #e5e5e5;border-radius:8px}" +
            "pre{white-space:pre-wrap;font-family:inherit}</style></head><body>" +
            "<h1>" + name + "</h1>" +
            "<p><strong>Headline:</strong> " + headline + "</p>" +
            "<p><strong>CTA:</strong> " + cta + "</p>" +
            assets + "</body></html>";
    }
}
