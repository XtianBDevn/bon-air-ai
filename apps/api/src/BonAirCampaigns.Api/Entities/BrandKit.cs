namespace BonAirCampaigns.Api.Entities;

public class BrandKit
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public string? LogoUrl { get; set; }
    public string PrimaryColor { get; set; } = "#6366f1";
    public string SecondaryColor { get; set; } = "#8b5cf6";
    public string AccentColor { get; set; } = "#06b6d4";
    public string FontFamily { get; set; } = "Inter";
    public string Tone { get; set; } = "professional";
    public string? Tagline { get; set; }
    public string? OpenAiApiKeyEncrypted { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
