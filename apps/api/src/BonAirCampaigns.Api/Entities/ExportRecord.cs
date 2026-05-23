namespace BonAirCampaigns.Api.Entities;

public class ExportRecord
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;
    public Guid OrganizationId { get; set; }
    public string Format { get; set; } = "pdf";
    public string? FileUrl { get; set; }
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
