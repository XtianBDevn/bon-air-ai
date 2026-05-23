using System.Text.Json;
using BonAirCampaigns.Api.Models;
using OpenAI.Chat;

namespace BonAirCampaigns.Api.Agents;

public interface ICampaignOrchestrator
{
    int LastTokensUsed { get; }
    Task<CampaignGenerationResult> RunAsync(CampaignInput input, CancellationToken ct = default);
}

public class CampaignOrchestrator(IConfiguration config, ILogger<CampaignOrchestrator> logger) : ICampaignOrchestrator
{
    public int LastTokensUsed { get; private set; }

    public async Task<CampaignGenerationResult> RunAsync(CampaignInput input, CancellationToken ct = default)
    {
        var apiKey = config["OpenAI:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogWarning("OpenAI API key not configured — using deterministic demo generation");
            LastTokensUsed = 3200;
            return DemoGenerator.Generate(input);
        }

        var client = new OpenAI.OpenAIClient(apiKey);
        var chat = client.GetChatClient(config["OpenAI:Model"] ?? "gpt-4o-mini");

        var systemPrompt = """
            You are Bon Air Media's campaign orchestration engine. Generate a complete multi-channel campaign as JSON.
            Return ONLY valid JSON matching this schema:
            {
              "headline": string,
              "cta": string,
              "facebookAds": [{"headline":"","primaryText":"","cta":""}],
              "googleAds": [{"headline1":"","headline2":"","description":""}],
              "emails": [{"subject":"","body":""}],
              "seo": {"title":"","metaDescription":"","keywords":[],"h1":"","sections":[{"heading":"","content":""}]},
              "landingPage": {
                "hero":"","subheadline":"","cta":"",
                "testimonials":[{"quote":"","author":"","role":""}],
                "pricing":[{"name":"","price":"","features":[]}],
                "faq":[{"question":"","answer":""}],
                "seoMetadata":{"title":"","description":""}
              },
              "gbpPosts": [string],
              "redditLaunch": string,
              "discordAnnouncement": string
            }
            Produce 3 facebook ads, 2 google ads, 3 emails, rich SEO, full landing page sections.
            """;

        var userPrompt = $"""
            Business type: {input.BusinessType}
            Offer: {input.Offer}
            Target audience: {input.TargetAudience}
            Location: {input.Location}
            Tone: {input.Tone}
            Budget: ${input.Budget ?? 0}
            Channels: {string.Join(", ", input.Channels)}
            """;

        var completion = await chat.CompleteChatAsync(
        [
            new SystemChatMessage(systemPrompt),
            new UserChatMessage(userPrompt)
        ],
        new ChatCompletionOptions
        {
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
        },
        ct);

        var text = completion.Value.Content[0].Text;
        LastTokensUsed = completion.Value.Usage?.TotalTokenCount ?? 4000;

        var result = JsonSerializer.Deserialize<CampaignGenerationResult>(text, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result ?? DemoGenerator.Generate(input);
    }
}

public static class DemoGenerator
{
    public static CampaignGenerationResult Generate(CampaignInput input)
    {
        var biz = FormatBusinessType(input.BusinessType);
        return new CampaignGenerationResult
        {
            Headline = $"{input.Offer} — Built for {biz} in {input.Location}",
            Cta = "Get Started Today",
            FacebookAds =
            [
                new AdVariant
                {
                    Headline = input.Offer,
                    PrimaryText = $"{biz} owners in {input.Location}: {input.TargetAudience} — limited time offer.",
                    Cta = "Learn More"
                },
                new AdVariant
                {
                    Headline = $"Trusted {biz} in {input.Location}",
                    PrimaryText = $"Join hundreds of satisfied customers. Tone: {input.Tone}.",
                    Cta = "Book Now"
                },
                new AdVariant
                {
                    Headline = "Don't Miss Out",
                    PrimaryText = $"{input.Offer}. Perfect for {input.TargetAudience}.",
                    Cta = "Claim Offer"
                }
            ],
            GoogleAds =
            [
                new GoogleAdVariant
                {
                    Headline1 = input.Offer[..Math.Min(30, input.Offer.Length)],
                    Headline2 = $"{biz} {input.Location}",
                    Description = $"Serving {input.TargetAudience}. Professional {input.Tone} service."
                },
                new GoogleAdVariant
                {
                    Headline1 = $"Best {biz} Near You",
                    Headline2 = "Book Online Today",
                    Description = input.Offer
                }
            ],
            Emails =
            [
                new EmailVariant
                {
                    Subject = $"{input.Offer} — exclusive for you",
                    Body = $"Hi {{{{first_name}}}},\n\nWe built this offer for {input.TargetAudience} in {input.Location}.\n\n{input.Offer}\n\nBook your spot today."
                },
                new EmailVariant
                {
                    Subject = "Last chance — offer ends soon",
                    Body = $"Don't wait — {input.Offer} won't last. Reply or click to claim."
                },
                new EmailVariant
                {
                    Subject = $"Why {biz} clients choose us",
                    Body = "Social proof, testimonials, and a clear CTA drive conversions. See what's possible."
                }
            ],
            Seo = new SeoContent
            {
                Title = $"{biz} in {input.Location} | {input.Offer}",
                MetaDescription = $"{input.Offer} for {input.TargetAudience} in {input.Location}.",
                Keywords = [biz.ToLower(), input.Location.ToLower(), input.Offer.ToLower()],
                H1 = input.Offer,
                Sections =
                [
                    new SeoSection { Heading = "About Us", Content = $"Leading {biz} serving {input.Location}." },
                    new SeoSection { Heading = "Our Offer", Content = input.Offer },
                    new SeoSection { Heading = "Who We Serve", Content = input.TargetAudience }
                ]
            },
            LandingPage = new LandingPageContent
            {
                Hero = input.Offer,
                Subheadline = $"For {input.TargetAudience} in {input.Location}",
                Cta = "Get Started",
                Testimonials =
                [
                    new Testimonial { Quote = "Transformed our marketing overnight.", Author = "Jordan M.", Role = "Agency Owner" },
                    new Testimonial { Quote = "Best ROI we've seen this quarter.", Author = "Sam K.", Role = "Restaurant Owner" }
                ],
                Pricing =
                [
                    new PricingTier { Name = "Starter", Price = "$49/mo", Features = ["10 campaigns", "Email support"] },
                    new PricingTier { Name = "Growth", Price = "$149/mo", Features = ["100 campaigns", "Priority support"] }
                ],
                Faq =
                [
                    new FaqItem { Question = "How fast can I launch?", Answer = "Most campaigns generate in under 2 minutes." },
                    new FaqItem { Question = "Can agencies white-label?", Answer = "Yes — Agency plan includes full white-label." }
                ],
                SeoMetadata = new SeoMetadata
                {
                    Title = $"{biz} | {input.Location}",
                    Description = input.Offer
                }
            },
            GbpPosts = [$"📍 {input.Location}: {input.Offer} — book today!", $"New offer for {input.TargetAudience}."],
            RedditLaunch = $"Launching in {input.Location}: {input.Offer} for {input.TargetAudience}. AMA about {biz}.",
            DiscordAnnouncement = $"🚀 **{input.Offer}** — now live for {input.TargetAudience} in {input.Location}!"
        };
    }

    private static string FormatBusinessType(string type) => type switch
    {
        "law_firm" => "Law Firm",
        "restaurant" => "Restaurant",
        "agency" => "Agency",
        "creator" => "Creator",
        "local_business" => "Local Business",
        _ => type.Replace('_', ' ')
    };
}
