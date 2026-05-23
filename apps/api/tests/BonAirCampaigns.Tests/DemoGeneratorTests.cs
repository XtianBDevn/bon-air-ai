using BonAirCampaigns.Api.Agents;
using Xunit;

namespace BonAirCampaigns.Tests;

public class DemoGeneratorTests
{
    [Fact]
    public void Generate_ReturnsCompleteCampaign()
    {
        var input = new CampaignInput
        {
            BusinessType = "law_firm",
            Offer = "Free consultation",
            TargetAudience = "Small business owners",
            Location = "Richmond, VA",
            Tone = "professional",
            Budget = 1000,
            Channels = ["facebook", "email"]
        };

        var result = DemoGenerator.Generate(input);

        Assert.False(string.IsNullOrWhiteSpace(result.Headline));
        Assert.NotEmpty(result.FacebookAds);
        Assert.NotEmpty(result.Emails);
        Assert.NotNull(result.Seo);
        Assert.NotNull(result.LandingPage);
    }
}
