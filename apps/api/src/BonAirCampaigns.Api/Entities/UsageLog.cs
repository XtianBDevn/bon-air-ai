namespace BonAirCampaigns.Api.Entities;

public class UsageLog
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;
    public Guid? UserId { get; set; }
    public Guid? CampaignId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public int TokensUsed { get; set; }
    public string? MetadataJson { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
