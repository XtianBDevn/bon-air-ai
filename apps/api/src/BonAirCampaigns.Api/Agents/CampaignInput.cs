namespace BonAirCampaigns.Api.Agents;

public class CampaignInput
{
    public string BusinessType { get; set; } = string.Empty;
    public string Offer { get; set; } = string.Empty;
    public string TargetAudience { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Tone { get; set; } = "professional";
    public decimal? Budget { get; set; }
    public List<string> Channels { get; set; } = new();
}
