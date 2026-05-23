namespace BonAirCampaigns.Api.Entities;

public class Organization
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string Plan { get; set; } = "starter";
    public string? StripeCustomerId { get; set; }
    public string? StripeSubscriptionId { get; set; }
    public string SubscriptionStatus { get; set; } = "trialing";
    public int CampaignsUsedThisMonth { get; set; }
    public int CampaignLimit { get; set; } = 10;
    public bool WhiteLabelEnabled { get; set; }
    public string? CustomDomain { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
    public ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();
    public BrandKit? BrandKit { get; set; }
}
