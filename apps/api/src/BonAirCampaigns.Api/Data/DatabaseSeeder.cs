using BonAirCampaigns.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace BonAirCampaigns.Api.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.EnsureCreatedAsync();

        if (await db.PromptTemplates.AnyAsync()) return;

        var templates = new[]
        {
            new PromptTemplate
            {
                Id = Guid.NewGuid(),
                AgentKey = "seo",
                Name = "SEO Agent",
                SystemPrompt = "You are an expert SEO strategist. Output structured JSON only.",
                UserPromptTemplate = "Create SEO strategy for {businessType} in {location} targeting {audience} with offer: {offer}. Tone: {tone}."
            },
            new PromptTemplate
            {
                Id = Guid.NewGuid(),
                AgentKey = "ad_copy",
                Name = "Ad Copy Agent",
                SystemPrompt = "You are a performance marketing copywriter for Facebook and Google ads.",
                UserPromptTemplate = "Write ad variants for {businessType}. Offer: {offer}. Audience: {audience}. Budget: {budget}. Tone: {tone}."
            },
            new PromptTemplate
            {
                Id = Guid.NewGuid(),
                AgentKey = "landing_page",
                Name = "Landing Page Agent",
                SystemPrompt = "You write high-converting landing page copy with hero, CTA, testimonials, pricing, FAQ.",
                UserPromptTemplate = "Landing page for {businessType} in {location}. Offer: {offer}. Audience: {audience}."
            },
            new PromptTemplate
            {
                Id = Guid.NewGuid(),
                AgentKey = "email",
                Name = "Email Agent",
                SystemPrompt = "You write email sequences for local businesses and agencies.",
                UserPromptTemplate = "Email campaign for {businessType}. Offer: {offer}. Audience: {audience}. Tone: {tone}."
            },
            new PromptTemplate
            {
                Id = Guid.NewGuid(),
                AgentKey = "analytics",
                Name = "Analytics Agent",
                SystemPrompt = "You provide campaign performance insights and KPI recommendations.",
                UserPromptTemplate = "Analytics recommendations for {businessType} campaign with budget {budget} across channels {channels}."
            }
        };

        db.PromptTemplates.AddRange(templates);

        var demoOrg = new Organization
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Bon Air Demo Agency",
            Slug = "bon-air-demo",
            Plan = "growth",
            CampaignLimit = 100,
            SubscriptionStatus = "active",
            CampaignsUsedThisMonth = 3
        };

        var demoUser = new AppUser
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            SupabaseUserId = "demo-supabase-user",
            Email = "demo@bonairmedia.com",
            FullName = "Alex Rivera",
            Role = "owner",
            OrganizationId = demoOrg.Id
        };

        var brandKit = new BrandKit
        {
            Id = Guid.NewGuid(),
            OrganizationId = demoOrg.Id,
            PrimaryColor = "#6366f1",
            SecondaryColor = "#8b5cf6",
            AccentColor = "#06b6d4",
            Tone = "bold",
            Tagline = "Campaigns that convert."
        };

        var campaign = new Campaign
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            OrganizationId = demoOrg.Id,
            CreatedByUserId = demoUser.Id,
            Name = "Spring Legal Consult Promo",
            Status = "completed",
            BusinessType = "law_firm",
            Offer = "Free 30-minute consultation",
            TargetAudience = "Small business owners in Richmond, VA",
            Location = "Richmond, VA",
            Tone = "trustworthy",
            Budget = 2500,
            ChannelsJson = "[\"facebook\",\"google\",\"email\",\"seo\"]",
            Headline = "Protect Your Business — Free Legal Consult",
            Cta = "Book Your Free Consultation",
            TokensUsed = 8420,
            ProgressPercent = 100,
            CompletedAt = DateTime.UtcNow.AddDays(-2),
            GeneratedJson = SampleCampaignJson
        };

        db.Organizations.Add(demoOrg);
        db.Users.Add(demoUser);
        db.BrandKits.Add(brandKit);
        db.Campaigns.Add(campaign);

        db.CampaignAssets.AddRange(
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = campaign.Id,
                AssetType = "facebook_ad",
                Title = "Facebook Ad — Trust",
                Content = "Worried about contracts? Richmond business owners trust Bon Air Legal. Free 30-min consult — no obligation."
            },
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = campaign.Id,
                AssetType = "email",
                Title = "Welcome Email",
                Content = "Subject: Your free legal consult is one click away\n\nHi {{first_name}},\n\nRunning a business means risk. Let's reduce yours — book a free 30-minute consultation this week."
            }
        );

        for (var i = 0; i < 14; i++)
        {
            db.UsageLogs.Add(new UsageLog
            {
                Id = Guid.NewGuid(),
                OrganizationId = demoOrg.Id,
                UserId = demoUser.Id,
                EventType = "ai_generation",
                TokensUsed = Random.Shared.Next(400, 1200),
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            });
        }

        await db.SaveChangesAsync();
    }

    private const string SampleCampaignJson = """
    {
      "headline": "Protect Your Business — Free Legal Consult",
      "cta": "Book Your Free Consultation",
      "facebookAds": [{"headline":"Free Legal Consult","primaryText":"Richmond business owners — protect what you've built.","cta":"Book Now"}],
      "googleAds": [{"headline1":"Free Legal Consult","headline2":"Richmond Business Lawyers","description":"30-minute consult. No obligation."}],
      "emails": [{"subject":"Your free legal consult","body":"Book this week — limited slots."}],
      "seo": {"title":"Business Lawyer Richmond VA","metaDescription":"Free consult for small business owners.","keywords":["business lawyer richmond"]},
      "landingPage": {"hero":"Protect Your Business","subheadline":"Free 30-minute consultation"},
      "gbpPosts": ["Free consult for Richmond business owners this month."],
      "redditLaunch": "We just launched free consults for local business owners in Richmond — happy to answer legal questions.",
      "discordAnnouncement": "📢 Free legal consult week for small business owners — DM for details."
    }
    """;
}
