namespace BonAirCampaigns.Api.Entities;

public class AppUser
{
    public Guid Id { get; set; }
    public string SupabaseUserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public string Role { get; set; } = "member";
    public bool IsPlatformAdmin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
}
