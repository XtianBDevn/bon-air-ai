using System.Text.Json.Serialization;

namespace BonAirCampaigns.Api.Models;

public class CampaignGenerationResult
{
    [JsonPropertyName("headline")]
    public string Headline { get; set; } = string.Empty;

    [JsonPropertyName("cta")]
    public string Cta { get; set; } = string.Empty;

    [JsonPropertyName("facebookAds")]
    public List<AdVariant> FacebookAds { get; set; } = new();

    [JsonPropertyName("googleAds")]
    public List<GoogleAdVariant> GoogleAds { get; set; } = new();

    [JsonPropertyName("emails")]
    public List<EmailVariant> Emails { get; set; } = new();

    [JsonPropertyName("seo")]
    public SeoContent Seo { get; set; } = new();

    [JsonPropertyName("landingPage")]
    public LandingPageContent LandingPage { get; set; } = new();

    [JsonPropertyName("gbpPosts")]
    public List<string> GbpPosts { get; set; } = new();

    [JsonPropertyName("redditLaunch")]
    public string RedditLaunch { get; set; } = string.Empty;

    [JsonPropertyName("discordAnnouncement")]
    public string DiscordAnnouncement { get; set; } = string.Empty;
}

public class AdVariant
{
    [JsonPropertyName("headline")]
    public string Headline { get; set; } = string.Empty;

    [JsonPropertyName("primaryText")]
    public string PrimaryText { get; set; } = string.Empty;

    [JsonPropertyName("cta")]
    public string Cta { get; set; } = string.Empty;
}

public class GoogleAdVariant
{
    [JsonPropertyName("headline1")]
    public string Headline1 { get; set; } = string.Empty;

    [JsonPropertyName("headline2")]
    public string Headline2 { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}

public class EmailVariant
{
    [JsonPropertyName("subject")]
    public string Subject { get; set; } = string.Empty;

    [JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;
}

public class SeoContent
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("metaDescription")]
    public string MetaDescription { get; set; } = string.Empty;

    [JsonPropertyName("keywords")]
    public List<string> Keywords { get; set; } = new();

    [JsonPropertyName("h1")]
    public string H1 { get; set; } = string.Empty;

    [JsonPropertyName("sections")]
    public List<SeoSection> Sections { get; set; } = new();
}

public class SeoSection
{
    [JsonPropertyName("heading")]
    public string Heading { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

public class LandingPageContent
{
    [JsonPropertyName("hero")]
    public string Hero { get; set; } = string.Empty;

    [JsonPropertyName("subheadline")]
    public string Subheadline { get; set; } = string.Empty;

    [JsonPropertyName("cta")]
    public string Cta { get; set; } = string.Empty;

    [JsonPropertyName("testimonials")]
    public List<Testimonial> Testimonials { get; set; } = new();

    [JsonPropertyName("pricing")]
    public List<PricingTier> Pricing { get; set; } = new();

    [JsonPropertyName("faq")]
    public List<FaqItem> Faq { get; set; } = new();

    [JsonPropertyName("seoMetadata")]
    public SeoMetadata SeoMetadata { get; set; } = new();
}

public class Testimonial
{
    [JsonPropertyName("quote")]
    public string Quote { get; set; } = string.Empty;

    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;
}

public class PricingTier
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public string Price { get; set; } = string.Empty;

    [JsonPropertyName("features")]
    public List<string> Features { get; set; } = new();
}

public class FaqItem
{
    [JsonPropertyName("question")]
    public string Question { get; set; } = string.Empty;

    [JsonPropertyName("answer")]
    public string Answer { get; set; } = string.Empty;
}

public class SeoMetadata
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}
