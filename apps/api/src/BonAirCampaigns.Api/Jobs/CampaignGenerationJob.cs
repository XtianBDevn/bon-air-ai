using BonAirCampaigns.Api.Services;

namespace BonAirCampaigns.Api.Jobs;

public class CampaignGenerationJob(ICampaignGenerationService generation)
{
    public Task Execute(Guid campaignId) => generation.GenerateAsync(campaignId);
}
