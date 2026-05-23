namespace BonAirCampaigns.Api.Entities;

public class Campaign
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public Guid CreatedByUserId { get; set; }
    public AppUser CreatedBy { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "draft";
    public string BusinessType { get; set; } = string.Empty;
    public string Offer { get; set; } = string.Empty;
    public string TargetAudience { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Tone { get; set; } = "professional";
    public decimal? Budget { get; set; }
    public string ChannelsJson { get; set; } = "[]";
    public string? GeneratedJson { get; set; }
    public string? Headline { get; set; }
    public string? Cta { get; set; }
    public int TokensUsed { get; set; }
    public int ProgressPercent { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<CampaignAsset> Assets { get; set; } = new List<CampaignAsset>();
}
