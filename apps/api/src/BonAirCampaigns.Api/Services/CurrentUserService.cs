using System.Security.Claims;
using BonAirCampaigns.Api.Data;
using BonAirCampaigns.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace BonAirCampaigns.Api.Services;

public interface ICurrentUserService
{
    string? SupabaseUserId { get; }
    Task<AppUser> GetOrCreateUserAsync(CancellationToken ct = default);
    Task<Organization> GetOrganizationAsync(CancellationToken ct = default);
}

public class CurrentUserService(IHttpContextAccessor http, AppDbContext db) : ICurrentUserService
{
    public string? SupabaseUserId =>
        http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? http.HttpContext?.User.FindFirstValue("sub");

    public async Task<AppUser> GetOrCreateUserAsync(CancellationToken ct = default)
    {
        var sub = SupabaseUserId ?? throw new UnauthorizedAccessException("Not authenticated");
        var existing = await db.Users.Include(u => u.Organization)
            .FirstOrDefaultAsync(u => u.SupabaseUserId == sub, ct);
        if (existing != null) return existing;

        var email = http.HttpContext?.User.FindFirstValue(ClaimTypes.Email)
            ?? http.HttpContext?.User.FindFirstValue("email")
            ?? $"{sub}@users.bonair.local";
        var name = http.HttpContext?.User.FindFirstValue("user_metadata.full_name")
            ?? http.HttpContext?.User.FindFirstValue("name");

        var orgName = email.Split('@')[0] + "'s Workspace";
        var slug = orgName.ToLowerInvariant().Replace(" ", "-") + "-" + Guid.NewGuid().ToString("N")[..6];
        var org = new Organization
        {
            Id = Guid.NewGuid(),
            Name = orgName,
            Slug = slug,
            Plan = "starter",
            CampaignLimit = 10,
            SubscriptionStatus = "trialing"
        };

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            SupabaseUserId = sub,
            Email = email,
            FullName = name,
            Role = "owner",
            OrganizationId = org.Id
        };

        db.Organizations.Add(org);
        db.Users.Add(user);
        db.BrandKits.Add(new BrandKit { Id = Guid.NewGuid(), OrganizationId = org.Id });
        await db.SaveChangesAsync(ct);
        return user;
    }

    public async Task<Organization> GetOrganizationAsync(CancellationToken ct = default)
    {
        var user = await GetOrCreateUserAsync(ct);
        return user.Organization ?? await db.Organizations.FirstAsync(o => o.Id == user.OrganizationId, ct);
    }
}
