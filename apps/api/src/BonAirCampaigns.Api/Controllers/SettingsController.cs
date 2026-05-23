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
public class SettingsController(AppDbContext db, ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet("brand-kit")]
    public async Task<ActionResult<BrandKitDto>> GetBrandKit(CancellationToken ct)
    {
        var org = await currentUser.GetOrganizationAsync(ct);
        var kit = await db.BrandKits.FirstOrDefaultAsync(b => b.OrganizationId == org.Id, ct);
        if (kit == null) return NotFound();
        return Map(kit);
    }

    [HttpPut("brand-kit")]
    public async Task<ActionResult<BrandKitDto>> UpdateBrandKit([FromBody] UpdateBrandKitRequest request, CancellationToken ct)
    {
        var org = await currentUser.GetOrganizationAsync(ct);
        var kit = await db.BrandKits.FirstOrDefaultAsync(b => b.OrganizationId == org.Id, ct);
        if (kit == null) return NotFound();

        kit.LogoUrl = request.LogoUrl;
        kit.PrimaryColor = request.PrimaryColor;
        kit.SecondaryColor = request.SecondaryColor;
        kit.AccentColor = request.AccentColor;
        kit.FontFamily = request.FontFamily;
        kit.Tone = request.Tone;
        kit.Tagline = request.Tagline;
        kit.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(ct);
        return Map(kit);
    }

    private static BrandKitDto Map(Entities.BrandKit k) => new(
        k.Id, k.LogoUrl, k.PrimaryColor, k.SecondaryColor,
        k.AccentColor, k.FontFamily, k.Tone, k.Tagline);
}
