using BonAirCampaigns.Api.Data;
using BonAirCampaigns.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BonAirCampaigns.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExportsController(IExportService exportService, AppDbContext db, ICurrentUserService currentUser, IWebHostEnvironment env) : ControllerBase
{
    [HttpPost("campaigns/{campaignId:guid}")]
    public async Task<ActionResult<object>> Export(Guid campaignId, [FromQuery] string format = "pdf", CancellationToken ct = default)
    {
        var org = await currentUser.GetOrganizationAsync(ct);
        var record = await exportService.ExportAsync(campaignId, format, org.Id, ct);
        return Ok(new { record.Id, record.Format, record.Status, record.FileUrl });
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken ct)
    {
        var user = await currentUser.GetOrCreateUserAsync(ct);
        var record = await db.Exports.FirstOrDefaultAsync(e => e.Id == id && e.OrganizationId == user.OrganizationId, ct);
        if (record == null || record.Status != "completed") return NotFound();

        var exportsDir = Path.Combine(env.ContentRootPath, "exports");
        var files = Directory.Exists(exportsDir)
            ? Directory.GetFiles(exportsDir, $"{record.CampaignId}-{record.Format}-*")
            : Array.Empty<string>();

        if (files.Length == 0) return NotFound();
        var file = files.OrderByDescending(f => f).First();
        var contentType = record.Format switch
        {
            "markdown" => "text/markdown",
            "html" => "text/html",
            _ => "application/pdf"
        };
        return PhysicalFile(file, contentType, Path.GetFileName(file));
    }
}
