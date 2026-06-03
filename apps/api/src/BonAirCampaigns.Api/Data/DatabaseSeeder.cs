using BonAirCampaigns.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace BonAirCampaigns.Api.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        await db.Database.EnsureCreatedAsync();

        if (await db.PromptTemplates.AnyAsync()) return;

        // ── Prompt Templates ──────────────────────────────────────────

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

        // ── Organization ──────────────────────────────────────────────

        var bonAirOrg = new Organization
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "Bon Air Media",
            Slug = "bon-air-media",
            Plan = "agency",
            CampaignLimit = 999,
            SubscriptionStatus = "active",
            CampaignsUsedThisMonth = 5
        };

        var christianUser = new AppUser
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            SupabaseUserId = "demo-supabase-user",
            Email = "christian.bryantrva@outlook.com",
            FullName = "Christian Bryant",
            Role = "owner",
            OrganizationId = bonAirOrg.Id
        };

        var brandKit = new BrandKit
        {
            Id = Guid.NewGuid(),
            OrganizationId = bonAirOrg.Id,
            PrimaryColor = "#6366f1",
            SecondaryColor = "#8b5cf6",
            AccentColor = "#06b6d4",
            Tone = "bold",
            Tagline = "AI-powered campaigns for agencies and local businesses."
        };

        db.Organizations.Add(bonAirOrg);
        db.Users.Add(christianUser);
        db.BrandKits.Add(brandKit);

        // ── Campaign 1: Alexander Law Office ──────────────────────────

        var alexanderCampaign = new Campaign
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            OrganizationId = bonAirOrg.Id,
            CreatedByUserId = christianUser.Id,
            Name = "Alexander Law Office — DUI Defense Campaign",
            Status = "completed",
            BusinessType = "law_firm",
            Offer = "Free 30-Minute DUI & Criminal Defense Consultation",
            TargetAudience = "Adults 21-55 facing DUI, reckless driving, or criminal charges in Richmond and Central Virginia",
            Location = "Richmond, VA",
            Tone = "trustworthy",
            Budget = 2500,
            ChannelsJson = "[\"facebook\",\"google\",\"email\",\"seo\",\"landing\",\"gbp\"]",
            Headline = "Protect Your Future — Free Legal Consultation with a Former Prosecutor",
            Cta = "Book Your Free Consultation",
            TokensUsed = 9240,
            ProgressPercent = 100,
            CompletedAt = DateTime.UtcNow.AddDays(-3),
            GeneratedJson = AlexanderCampaignJson
        };

        db.Campaigns.Add(alexanderCampaign);

        db.CampaignAssets.AddRange(
            // ── Facebook Ads ──
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = alexanderCampaign.Id,
                AssetType = "facebook_ad",
                Title = "Facebook Ad 1 — Trust & Authority",
                Content = "Charged with DUI in Richmond?\n\n30+ years of courtroom experience. Former prosecutor. 10.0 Superb AVVO rating. A+ BBB rating.\n\nH. Standish Alexander III provides aggressive, effective defense for DUI, reckless driving, and criminal charges in Richmond and Central Virginia.\n\nFree 30-minute consultation — no obligation.\n\nCTA: Book Your Free Consultation"
            },
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = alexanderCampaign.Id,
                AssetType = "facebook_ad",
                Title = "Facebook Ad 2 — Reckless Driving",
                Content = "Going 81 in a 65 in Virginia? That's not a speeding ticket — it's a CRIME.\n\nVirginia treats reckless driving as a Class 1 misdemeanor. You could face jail time, fines up to $2,500, and a criminal record.\n\nDon't face it alone. Alexander Law Office has defended Richmond drivers for 30+ years.\n\nCTA: Free Case Evaluation"
            },
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = alexanderCampaign.Id,
                AssetType = "facebook_ad",
                Title = "Facebook Ad 3 — Retargeting / Urgency",
                Content = "Still thinking about your case? Every day matters.\n\nVirginia DUI penalties include mandatory license suspension, heavy fines, and possible jail time — even for a first offense.\n\nA former prosecutor knows how these cases are built — and how to fight them.\n\nFree consultation. Call (804) 814-1489 today.\n\nCTA: Call Now"
            },
            // ── Google Ads ──
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = alexanderCampaign.Id,
                AssetType = "google_ad",
                Title = "Google Ad 1 — DUI Defense",
                Content = "Richmond DUI Lawyer — Free Consult | 30+ Years Experience\nFormer Prosecutor Defending DUI, Reckless Driving & Criminal Charges in Richmond VA. Call (804) 814-1489."
            },
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = alexanderCampaign.Id,
                AssetType = "google_ad",
                Title = "Google Ad 2 — Reckless Driving",
                Content = "Reckless Driving? Fight Back | Alexander Law Office\nCharged with reckless driving in Virginia? A+ BBB. 10.0 AVVO. Free 30-min consultation. Call today."
            },
            // ── Emails ──
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = alexanderCampaign.Id,
                AssetType = "email",
                Title = "Email 1: Lead Magnet Delivery",
                Content = "Subject: Your Virginia DUI Defense Guide is here\n\nHi {{first_name}},\n\nThank you for downloading our Virginia DUI Defense Guide — 10 essential things you need to know if you've been charged.\n\nInside you'll find:\n• What happens after a DUI arrest in Virginia\n• Your rights during a traffic stop\n• Virginia DUI penalties at a glance\n• 5 mistakes that can hurt your case\n• When to hire a lawyer (hint: immediately)\n\nH. Standish Alexander III has spent 30+ years defending Richmond drivers — including time as a prosecutor. He knows how these cases are built, and more importantly, how to fight them.\n\nReady to discuss your case? Book a free 30-minute consultation:\n[BOOK NOW]\n\nAlexander Law Office, P.C.\n(804) 814-1489"
            },
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = alexanderCampaign.Id,
                AssetType = "email",
                Title = "Email 2: Social Proof & Nudge",
                Content = "Subject: What our clients say about working with us\n\nHi {{first_name}},\n\nWhen you're facing criminal charges, you want an attorney who fights for you — not one who settles.\n\nHere's what recent clients have said:\n\n\"Mr. Alexander got my reckless driving charge reduced to a simple speeding ticket. I could have lost my license.\" — J.R., Henrico County\n\n\"He was honest, direct, and prepared. Best decision I made was calling his office first.\" — T.M., Richmond\n\nIf you're facing DUI, reckless driving, or criminal charges in Richmond or Central Virginia, your free consultation is waiting.\n\n[SCHEDULE YOUR FREE CONSULTATION]\n\nAlexander Law Office, P.C.\n1000 Greenway Ln, Richmond, VA 23226"
            },
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = alexanderCampaign.Id,
                AssetType = "email",
                Title = "Email 3: Urgency & Deadline",
                Content = "Subject: Don't wait — Virginia DUI deadlines are real\n\nHi {{first_name}},\n\nIn Virginia, you have a limited window to challenge your charges and protect your driving privileges.\n\nWaiting too long can mean:\n• Automatic license suspension\n• Missed opportunities for reduced charges\n• Increased penalties at sentencing\n\nMr. Alexander has successfully defended thousands of cases in Richmond courts. As a former prosecutor, he knows exactly what the other side is planning.\n\nYour free 30-minute consultation costs nothing — and could save everything.\n\n[BOOK YOUR FREE CONSULTATION NOW]\n\nAlexander Law Office, P.C.\n(804) 814-1489"
            },
            // ── SEO ──
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = alexanderCampaign.Id,
                AssetType = "seo",
                Title = "SEO Package — Alexander Law Office",
                Content = "{\n  \"title\": \"Richmond DUI Lawyer | Alexander Law Office | Free Consultation (804) 814-1489\",\n  \"metaDescription\": \"30+ years defending DUI, reckless driving & criminal charges in Richmond VA. Former prosecutor. 10.0 AVVO. A+ BBB. Free consultation — call (804) 814-1489.\",\n  \"keywords\": [\"richmond dui lawyer\", \"dui attorney richmond va\", \"reckless driving lawyer richmond\", \"criminal defense attorney richmond va\", \"dui defense henrico\", \"reckless driving chesterfield\", \"free legal consultation richmond\", \"expungement lawyer virginia\"],\n  \"h1\": \"Experienced DUI & Criminal Defense Lawyer in Richmond, VA\",\n  \"sections\": [\n    {\"heading\": \"Why Choose Alexander Law Office\", \"content\": \"H. Standish Alexander III brings 30+ years of courtroom experience as both a prosecutor and defense attorney. With a 10.0 Superb AVVO rating and A+ BBB rating, he provides aggressive, effective defense for DUI, reckless driving, criminal charges, traffic violations, and expungement cases across Richmond, Henrico, Chesterfield, and Central Virginia.\"},\n    {\"heading\": \"Practice Areas\", \"content\": \"DUI Defense • Reckless Driving • Criminal Defense • Traffic Violations • Expungement. Each practice area is backed by decades of courtroom experience and a former prosecutor's insight into how the state builds its cases.\"},\n    {\"heading\": \"Serving Richmond & Central Virginia\", \"content\": \"Located at 1000 Greenway Ln, Richmond, VA 23226. Serving clients in Richmond City, Henrico County, Chesterfield County, Hanover County, and throughout Central Virginia. Free consultations available — call (804) 814-1489.\"},\n    {\"heading\": \"DUI Defense FAQ\", \"content\": \"What are Virginia DUI penalties? First-offense DUI carries mandatory license suspension, fines up to $2,500, and possible jail time. Virginia treats reckless driving at 20+ mph over the limit as a Class 1 misdemeanor — the same level as DUI. Contact us immediately if you've been charged.\"},\n    {\"heading\": \"New Location Pages Needed\", \"content\": \"Expand to cover /dui-lawyer-henrico-va/, /dui-lawyer-chesterfield-va/, /reckless-driving-henrico/, /first-offense-dui-virginia/, and /virginia-reckless-driving-speed/ for maximum local SEO coverage.\"}\n  ]\n}"
            },
            // ── Landing Page ──
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = alexanderCampaign.Id,
                AssetType = "landing_page",
                Title = "Landing Page Copy — Alexander Law Office",
                Content = "{\n  \"hero\": \"Charged with DUI or Reckless Driving in Richmond?\",\n  \"subheadline\": \"A former prosecutor with 30+ years of courtroom experience is ready to fight for you. Free 30-minute consultation.\",\n  \"cta\": \"Book Your Free Consultation\",\n  \"testimonials\": [\n    {\"quote\": \"Mr. Alexander got my reckless driving charge reduced to a simple speeding ticket. I could have lost my license and my job.\", \"author\": \"J.R.\", \"role\": \"Henrico County Client\"},\n    {\"quote\": \"He was honest, direct, and prepared. Best decision I made was calling his office first.\", \"author\": \"T.M.\", \"role\": \"Richmond Client\"},\n    {\"quote\": \"As a small business owner, a DUI conviction would have ended everything. Mr. Alexander understood what was at stake.\", \"author\": \"S.K.\", \"role\": \"Chesterfield Business Owner\"}\n  ],\n  \"pricing\": [\n    {\"name\": \"Free Consultation\", \"price\": \"$0\", \"features\": [\"30-minute case review\", \"No obligation\", \"Same-week availability\", \"In-person or phone\"]},\n    {\"name\": \"DUI Defense\", \"price\": \"Contact for quote\", \"features\": [\"Full case investigation\", \"Court representation\", \"DMV hearing support\", \"Former prosecutor strategy\"]},\n    {\"name\": \"Full Criminal Defense\", \"price\": \"Contact for quote\", \"features\": [\"All criminal charges\", \"Expungement services\", \"Appeal representation\", \"Payment plans available\"]}\n  ],\n  \"faq\": [\n    {\"question\": \"What should I do if I'm charged with DUI in Virginia?\", \"answer\": \"Contact an attorney immediately. Virginia DUI carries mandatory license suspension and potential jail time — even for a first offense. The sooner you have legal representation, the stronger your defense.\"},\n    {\"question\": \"Is reckless driving really a criminal offense in Virginia?\", \"answer\": \"Yes. Virginia is one of the strictest states — driving 20+ mph over the limit or over 85 mph anywhere is a Class 1 misdemeanor, punishable by up to 12 months in jail and a $2,500 fine.\"},\n    {\"question\": \"How is a former prosecutor different as a defense attorney?\", \"answer\": \"Mr. Alexander knows how the prosecution builds its case because he used to build them. This means he can identify weaknesses, challenge evidence, and negotiate from a position of deep knowledge.\"},\n    {\"question\": \"Do you handle cases outside of Richmond?\", \"answer\": \"Yes. We serve clients across Henrico County, Chesterfield County, Hanover County, and all of Central Virginia.\"}\n  ],\n  \"seoMetadata\": {\"title\": \"Richmond DUI Lawyer | Free Consultation | Alexander Law Office\", \"description\": \"Facing DUI or criminal charges in Richmond, VA? 30+ years experience. Former prosecutor. Free 30-minute consultation. Call (804) 814-1489.\"}\n}"
            },
            // ── GBP Posts ──
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = alexanderCampaign.Id,
                AssetType = "gbp",
                Title = "GBP Posts — Alexander Law Office",
                Content = "Post 1: Charged with reckless driving in Virginia? Speeds 20+ mph over the limit can result in a Class 1 misdemeanor — not just a traffic ticket. Call (804) 814-1489 for a free consultation.\n\nPost 2: Did you know? First-offense DUI in Virginia carries a mandatory license suspension. Learn your options — free consultation available at Alexander Law Office.\n\nPost 3: Over 30 years defending Richmond drivers. Former prosecutor. 10.0 AVVO rating. If you're facing DUI, reckless driving, or criminal charges — we can help. Call today."
            }
        );

        // ── Campaign 2: Cowboy Tony's Doughnuts ──────────────────────

        var cowboyTonysCampaign = new Campaign
        {
            Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            OrganizationId = bonAirOrg.Id,
            CreatedByUserId = christianUser.Id,
            Name = "Cowboy Tony's — The Great Doughnut Drop",
            Status = "completed",
            BusinessType = "restaurant",
            Offer = "300 Free Doughnuts Giveaway + Social Media Amplification",
            TargetAudience = "Foodies, local community members, and social media followers ages 18-45 in the local area",
            Location = "Richmond, VA",
            Tone = "playful",
            Budget = 6000,
            ChannelsJson = "[\"facebook\",\"email\",\"seo\",\"landing\",\"gbp\",\"reddit\",\"discord\"]",
            Headline = "300 Free Doughnuts — One Lucky Signup Away",
            Cta = "Enter the Great Doughnut Drop",
            TokensUsed = 10850,
            ProgressPercent = 100,
            CompletedAt = DateTime.UtcNow.AddDays(-2),
            GeneratedJson = CowboyTonysCampaignJson
        };

        db.Campaigns.Add(cowboyTonysCampaign);

        db.CampaignAssets.AddRange(
            // ── Facebook Ads ──
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = cowboyTonysCampaign.Id,
                AssetType = "facebook_ad",
                Title = "Facebook Ad 1 — Giveaway Launch",
                Content = "🍩 300 FREE DOUGHNUTS. Yes, you read that right.\n\nCowboy Tony's is giving away 300 doughnuts in The Great Doughnut Drop!\n\nAll you need to do:\n1. Sign up with your email\n2. Follow us on social\n3. Cross your fingers 🤞\n\nNon-winners get a BOGO coupon — so everyone wins.\n\n⏰ Limited time only.\n\nCTA: Enter Now"
            },
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = cowboyTonysCampaign.Id,
                AssetType = "facebook_ad",
                Title = "Facebook Ad 2 — Social Proof / FOMO",
                Content = "Over 500 people have already entered The Great Doughnut Drop 🤠🍩\n\nHave you?\n\n300 lucky winners get FREE doughnuts from Cowboy Tony's. Everyone else gets a BOGO coupon.\n\nIt takes 10 seconds to enter. Don't miss out.\n\nCTA: Claim Your Entry"
            },
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = cowboyTonysCampaign.Id,
                AssetType = "facebook_ad",
                Title = "Facebook Ad 3 — Last Chance",
                Content = "⏰ LAST CHANCE — The Great Doughnut Drop closes TOMORROW!\n\n300 free doughnuts. Hundreds of entries. Your odds are still good.\n\nEnter now before it's too late. Non-winners still get a special coupon. 🍩\n\nCTA: Enter Before Midnight"
            },
            // ── Google Ads ──
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = cowboyTonysCampaign.Id,
                AssetType = "google_ad",
                Title = "Google Ad 1 — Giveaway",
                Content = "300 Free Doughnuts | Cowboy Tony's Giveaway\nEnter The Great Doughnut Drop — sign up for a chance to win free doughnuts. Everyone gets a coupon!"
            },
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = cowboyTonysCampaign.Id,
                AssetType = "google_ad",
                Title = "Google Ad 2 — Brand",
                Content = "Cowboy Tony's Doughnuts | Fresh Daily\nHandcrafted doughnuts with a cowboy twist. Follow us on social for new flavors, behind-the-scenes, and giveaways."
            },
            // ── Emails ──
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = cowboyTonysCampaign.Id,
                AssetType = "email",
                Title = "Email 1: The Teaser (Day -14)",
                Content = "Subject: Something BIG is coming from Cowboy Tony's 🤠🍩\n\nHi {{first_name}},\n\nWe've been cooking up something special. Something sweet. Something… massive.\n\nAll we can say right now: it involves doughnuts. A LOT of doughnuts.\n\nStay tuned. You'll want to be first in line for this one.\n\n🤠 Cowboy Tony's Doughnuts\n\nP.S. Share this email with a friend who loves doughnuts. They'll thank you later."
            },
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = cowboyTonysCampaign.Id,
                AssetType = "email",
                Title = "Email 2: The Announcement (Day -7)",
                Content = "Subject: 300 FREE Doughnuts — Enter Now!\n\nHi {{first_name}},\n\n🎉 It's official: The Great Doughnut Drop is HERE!\n\nWe're giving away 300 FREE DOUGHNUTS to lucky winners.\n\nHow to enter:\n1. Click the link below to sign up\n2. Follow Cowboy Tony's on Facebook, Instagram, or TikTok\n3. Bonus entries: share with friends or post about it!\n\n[ENTER THE GREAT DOUGHNUT DROP]\n\n300 winners. Free doughnuts. Non-winners get a BOGO coupon — so everyone walks away happy.\n\n⏰ Deadline: [DATE] at midnight.\n\n🤠 Cowboy Tony's Doughnuts"
            },
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = cowboyTonysCampaign.Id,
                AssetType = "email",
                Title = "Email 3: Social Proof + Urgency (Day -3)",
                Content = "Subject: Over 500 people already entered — have you?\n\nHi {{first_name}},\n\nThe Great Doughnut Drop is blowing up:\n\n✅ 500+ entries and counting\n✅ Trending on local social media\n✅ Only [X] days left to enter\n\nDon't miss your chance at free doughnuts.\n\n[ENTER NOW — TAKES 10 SECONDS]\n\nBonus: Share on social for extra entries! Use #CowboyTonys 🤠\n\n🤠 Cowboy Tony's Doughnuts"
            },
            // ── SEO ──
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = cowboyTonysCampaign.Id,
                AssetType = "seo",
                Title = "SEO Package — Cowboy Tony's Doughnuts",
                Content = "{\n  \"title\": \"Cowboy Tony's Doughnuts | Handcrafted Fresh Daily\",\n  \"metaDescription\": \"Cowboy Tony's Doughnuts — handcrafted, fresh daily with a cowboy twist. Order online, check today's flavors, and follow us for giveaways and new drops.\",\n  \"keywords\": [\"cowboy tonys doughnuts\", \"doughnuts near me\", \"fresh doughnuts\", \"best doughnuts richmond\", \"gourmet doughnuts\", \"doughnut giveaway\", \"artisan doughnuts\", \"local bakery\"],\n  \"h1\": \"Handcrafted Doughnuts with a Cowboy Twist\",\n  \"sections\": [\n    {\"heading\": \"Our Story\", \"content\": \"Cowboy Tony's Doughnuts brings bold flavors and handcrafted quality to every batch. From classic glazed to limited-edition seasonal drops, every doughnut is made fresh daily with premium ingredients.\"},\n    {\"heading\": \"Today's Flavors\", \"content\": \"Check our social media for today's flavor lineup. New drops every week — from Maple Bourbon Pecan to Strawberry Lemonade Glaze. Follow @cowboytonys for first access to limited editions.\"},\n    {\"heading\": \"The Great Doughnut Drop\", \"content\": \"Enter our 300-doughnut giveaway! Sign up with your email for a chance to win free doughnuts. Non-winners receive a BOGO coupon. Follow us on Facebook, Instagram, and TikTok for bonus entries.\"},\n    {\"heading\": \"Catering & Events\", \"content\": \"Planning a party, office event, or wedding? Cowboy Tony's offers catering packages with custom flavors and branded boxes. Contact us for a quote.\"},\n    {\"heading\": \"Find Us\", \"content\": \"Visit us in person or order online. Check Google Maps for hours and directions. Tag your photos with #CowboyTonys!\"}\n  ]\n}"
            },
            // ── Landing Page ──
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = cowboyTonysCampaign.Id,
                AssetType = "landing_page",
                Title = "Landing Page Copy — The Great Doughnut Drop",
                Content = "{\n  \"hero\": \"300 Free Doughnuts. One Lucky Signup Away.\",\n  \"subheadline\": \"Enter The Great Doughnut Drop from Cowboy Tony's Doughnuts. Sign up, follow us, and you could win free handcrafted doughnuts.\",\n  \"cta\": \"Enter the Giveaway\",\n  \"testimonials\": [\n    {\"quote\": \"Best doughnuts I've ever had. The Maple Bourbon Pecan changed my life.\", \"author\": \"Sarah M.\", \"role\": \"Loyal Customer\"},\n    {\"quote\": \"I drive 30 minutes just for the weekend drops. Worth every mile.\", \"author\": \"Marcus T.\", \"role\": \"Weekend Regular\"},\n    {\"quote\": \"Ordered 5 dozen for our office party. They were gone in 20 minutes.\", \"author\": \"Kim L.\", \"role\": \"Corporate Client\"}\n  ],\n  \"pricing\": [\n    {\"name\": \"Free Entry\", \"price\": \"$0\", \"features\": [\"Chance to win 1 free doughnut\", \"BOGO coupon for non-winners\", \"Email signup only\"]},\n    {\"name\": \"Bonus Entry\", \"price\": \"$0\", \"features\": [\"Extra chances to win\", \"Share on social media\", \"Tag a friend\", \"Post a TikTok\"]},\n    {\"name\": \"Top Engager Prize\", \"price\": \"$0\", \"features\": [\"Win a free half-dozen box\", \"Most social shares\", \"Featured on our page\"]}\n  ],\n  \"faq\": [\n    {\"question\": \"How do I enter The Great Doughnut Drop?\", \"answer\": \"Sign up with your email on this page and follow Cowboy Tony's on at least one social platform (Facebook, Instagram, or TikTok). Optional: share the giveaway for bonus entries.\"},\n    {\"question\": \"What if I don't win?\", \"answer\": \"Every non-winner receives a BOGO coupon redeemable in-store. So everyone walks away with something sweet.\"},\n    {\"question\": \"When are winners announced?\", \"answer\": \"Winners are selected by random drawing after the campaign window closes and announced via email + social media.\"},\n    {\"question\": \"Can I enter more than once?\", \"answer\": \"One email entry per person, but you can earn bonus entries by sharing the giveaway, tagging friends, or posting on TikTok.\"}\n  ],\n  \"seoMetadata\": {\"title\": \"The Great Doughnut Drop | Cowboy Tony's Doughnuts — 300 Free Doughnuts\", \"description\": \"Enter for a chance to win free doughnuts from Cowboy Tony's. 300 winners. Non-winners get a BOGO coupon. Sign up now!\"}\n}"
            },
            // ── Reddit ──
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = cowboyTonysCampaign.Id,
                AssetType = "reddit",
                Title = "Reddit Launch Post",
                Content = "Title: Cowboy Tony's is giving away 300 FREE doughnuts — The Great Doughnut Drop is live\n\nBody: Hey everyone — Cowboy Tony's Doughnuts is running our biggest giveaway ever. 300 free doughnuts to random winners. All you have to do is sign up with your email and follow us on social.\n\nNon-winners get a BOGO coupon so nobody goes home empty-handed.\n\nWe handcraft everything fresh daily — think Maple Bourbon Pecan, Strawberry Lemonade Glaze, and classic cowboy-style glazed.\n\nLink in comments. Happy to answer questions about the giveaway or our flavors! 🤠🍩"
            },
            // ── GBP Posts ──
            new CampaignAsset
            {
                Id = Guid.NewGuid(),
                CampaignId = cowboyTonysCampaign.Id,
                AssetType = "gbp",
                Title = "GBP Posts — Cowboy Tony's",
                Content = "Post 1: 🍩 The Great Doughnut Drop is LIVE! We're giving away 300 free doughnuts. Sign up on our website — link in profile. Non-winners get a BOGO coupon!\n\nPost 2: New flavor drop this week! Come try our limited-edition seasonal doughnuts — handcrafted fresh daily. Follow us on social for first access.\n\nPost 3: Looking for catering? Cowboy Tony's offers custom doughnut boxes for parties, office events, and weddings. DM us or call for a quote! 🤠"
            }
        );

        // ── Usage Logs (realistic data for dashboard) ─────────────────

        for (var i = 0; i < 21; i++)
        {
            db.UsageLogs.Add(new UsageLog
            {
                Id = Guid.NewGuid(),
                OrganizationId = bonAirOrg.Id,
                UserId = christianUser.Id,
                CampaignId = i < 10 ? alexanderCampaign.Id : (i < 20 ? cowboyTonysCampaign.Id : null),
                EventType = "campaign_generated",
                TokensUsed = Random.Shared.Next(800, 2400),
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            });
        }

        await db.SaveChangesAsync();
    }

    // ── Alexander Law Office — Full Campaign JSON ─────────────────────

    private const string AlexanderCampaignJson = """
    {
      "headline": "Protect Your Future — Free Legal Consultation with a Former Prosecutor",
      "cta": "Book Your Free Consultation",
      "facebookAds": [
        {
          "headline": "Charged with DUI in Richmond?",
          "primaryText": "30+ years of courtroom experience. Former prosecutor. 10.0 Superb AVVO rating. Free 30-minute consultation — no obligation.",
          "cta": "Book Your Free Consultation"
        },
        {
          "headline": "Going 81 in a 65? That's a Crime in Virginia.",
          "primaryText": "Virginia treats reckless driving as a Class 1 misdemeanor. You could face jail time, fines up to $2,500, and a criminal record. Don't face it alone.",
          "cta": "Free Case Evaluation"
        },
        {
          "headline": "Every Day Matters When You're Facing Charges",
          "primaryText": "Virginia DUI penalties include mandatory license suspension, heavy fines, and possible jail time — even for a first offense. A former prosecutor knows how to fight back.",
          "cta": "Call Now"
        }
      ],
      "googleAds": [
        {
          "headline1": "Richmond DUI Lawyer",
          "headline2": "Free 30-Min Consult",
          "description": "Former Prosecutor. 30+ Years. 10.0 AVVO. A+ BBB. Defending DUI, reckless driving & criminal charges in Richmond VA. (804) 814-1489."
        },
        {
          "headline1": "Reckless Driving? Fight Back",
          "headline2": "Alexander Law Office",
          "description": "Charged with reckless driving in Virginia? A+ BBB. 10.0 AVVO. Free 30-min consultation. Call today."
        }
      ],
      "emails": [
        {
          "subject": "Your Virginia DUI Defense Guide is here",
          "body": "Hi {{first_name}},\n\nThank you for downloading our Virginia DUI Defense Guide. Inside: what happens after an arrest, your rights during a traffic stop, penalties at a glance, and 5 mistakes that can hurt your case.\n\nH. Standish Alexander III has 30+ years defending Richmond drivers. Book a free 30-minute consultation.\n\nAlexander Law Office, P.C. | (804) 814-1489"
        },
        {
          "subject": "What our clients say about working with us",
          "body": "Hi {{first_name}},\n\nWhen facing criminal charges, you want someone who fights — not settles.\n\n\"Got my reckless driving reduced to a speeding ticket.\" — J.R., Henrico\n\"Honest, direct, prepared.\" — T.M., Richmond\n\nYour free consultation is waiting.\n\nAlexander Law Office, P.C."
        },
        {
          "subject": "Don't wait — Virginia DUI deadlines are real",
          "body": "Hi {{first_name}},\n\nYou have a limited window to challenge your charges. Waiting can mean automatic license suspension and missed opportunities for reduced charges.\n\nAs a former prosecutor, Mr. Alexander knows exactly what the other side is planning. Free 30-minute consultation.\n\n(804) 814-1489"
        }
      ],
      "seo": {
        "title": "Richmond DUI Lawyer | Alexander Law Office | Free Consultation (804) 814-1489",
        "metaDescription": "30+ years defending DUI, reckless driving & criminal charges in Richmond VA. Former prosecutor. 10.0 AVVO. A+ BBB. Free consultation.",
        "keywords": ["richmond dui lawyer", "dui attorney richmond va", "reckless driving lawyer richmond", "criminal defense attorney richmond va", "dui defense henrico", "free legal consultation richmond"],
        "h1": "Experienced DUI & Criminal Defense Lawyer in Richmond, VA",
        "sections": [
          {"heading": "Why Choose Alexander Law Office", "content": "30+ years as prosecutor and defense attorney. 10.0 AVVO. A+ BBB. Aggressive defense for DUI, reckless driving, criminal charges, traffic violations, and expungement."},
          {"heading": "Practice Areas", "content": "DUI Defense • Reckless Driving • Criminal Defense • Traffic Violations • Expungement"},
          {"heading": "Serving Richmond & Central Virginia", "content": "1000 Greenway Ln, Richmond, VA 23226. Serving Richmond, Henrico, Chesterfield, Hanover. Free consultations — (804) 814-1489."},
          {"heading": "DUI FAQ", "content": "First-offense DUI carries mandatory license suspension and fines up to $2,500. Reckless driving at 20+ mph over is a Class 1 misdemeanor. Contact us immediately."}
        ]
      },
      "landingPage": {
        "hero": "Charged with DUI or Reckless Driving in Richmond?",
        "subheadline": "A former prosecutor with 30+ years of experience is ready to fight for you.",
        "cta": "Book Your Free Consultation",
        "testimonials": [
          {"quote": "Got my reckless driving charge reduced to a speeding ticket.", "author": "J.R.", "role": "Henrico County Client"},
          {"quote": "Honest, direct, and prepared. Best decision I made.", "author": "T.M.", "role": "Richmond Client"},
          {"quote": "A DUI conviction would have ended everything. Mr. Alexander understood.", "author": "S.K.", "role": "Chesterfield Business Owner"}
        ],
        "pricing": [
          {"name": "Free Consultation", "price": "$0", "features": ["30-minute case review", "No obligation", "Same-week availability"]},
          {"name": "DUI Defense", "price": "Contact for quote", "features": ["Full investigation", "Court representation", "DMV hearing support"]},
          {"name": "Full Criminal Defense", "price": "Contact for quote", "features": ["All charges", "Expungement", "Payment plans"]}
        ],
        "faq": [
          {"question": "What should I do if charged with DUI?", "answer": "Contact an attorney immediately. Virginia DUI carries mandatory license suspension and potential jail time."},
          {"question": "Is reckless driving really criminal?", "answer": "Yes. 20+ mph over the limit is a Class 1 misdemeanor — same level as DUI. Up to 12 months jail and $2,500 fine."},
          {"question": "How is a former prosecutor different?", "answer": "Mr. Alexander knows how the prosecution builds its case. He identifies weaknesses and negotiates from deep knowledge."}
        ],
        "seoMetadata": {"title": "Richmond DUI Lawyer | Free Consultation | Alexander Law Office", "description": "Facing DUI or criminal charges in Richmond? 30+ years. Former prosecutor. Free consultation. (804) 814-1489."}
      },
      "gbpPosts": [
        "Charged with reckless driving in Virginia? Speeds 20+ mph over the limit are a Class 1 misdemeanor. Call (804) 814-1489 for a free consultation.",
        "First-offense DUI in Virginia carries mandatory license suspension. Learn your options — free consultation available.",
        "30+ years defending Richmond drivers. Former prosecutor. 10.0 AVVO. Free consultation — call today."
      ],
      "redditLaunch": "Former prosecutor turned defense attorney in Richmond, VA — offering free 30-minute consultations for DUI, reckless driving, and criminal charges. 30+ years of experience. Happy to answer general Virginia law questions here.",
      "discordAnnouncement": "⚖️ **Free Legal Consultation** — Alexander Law Office in Richmond, VA. 30+ years defending DUI, reckless driving & criminal charges. Former prosecutor. Call (804) 814-1489 or book online."
    }
    """;

    // ── Cowboy Tony's Doughnuts — Full Campaign JSON ──────────────────

    private const string CowboyTonysCampaignJson = """
    {
      "headline": "300 Free Doughnuts — One Lucky Signup Away",
      "cta": "Enter the Great Doughnut Drop",
      "facebookAds": [
        {
          "headline": "300 FREE DOUGHNUTS 🍩",
          "primaryText": "Cowboy Tony's is giving away 300 doughnuts in The Great Doughnut Drop! Sign up, follow us, cross your fingers. Non-winners get a BOGO coupon — everyone wins.",
          "cta": "Enter Now"
        },
        {
          "headline": "500+ People Already Entered",
          "primaryText": "The Great Doughnut Drop is blowing up. 300 lucky winners get free doughnuts from Cowboy Tony's. 10 seconds to enter. Don't miss out.",
          "cta": "Claim Your Entry"
        },
        {
          "headline": "⏰ LAST CHANCE",
          "primaryText": "The Great Doughnut Drop closes TOMORROW. 300 free doughnuts. Your odds are still good. Non-winners get a special coupon. Enter before midnight!",
          "cta": "Enter Before Midnight"
        }
      ],
      "googleAds": [
        {
          "headline1": "300 Free Doughnuts",
          "headline2": "Cowboy Tony's Giveaway",
          "description": "Enter The Great Doughnut Drop — sign up for a chance to win free handcrafted doughnuts. Everyone gets a coupon!"
        },
        {
          "headline1": "Cowboy Tony's Doughnuts",
          "headline2": "Fresh Daily",
          "description": "Handcrafted doughnuts with a cowboy twist. Follow us for new flavors, behind-the-scenes, and giveaways."
        }
      ],
      "emails": [
        {
          "subject": "Something BIG is coming from Cowboy Tony's 🤠🍩",
          "body": "We've been cooking up something special. Something sweet. Something massive. All we can say: it involves doughnuts. A LOT of doughnuts. Stay tuned."
        },
        {
          "subject": "300 FREE Doughnuts — Enter Now!",
          "body": "The Great Doughnut Drop is HERE! 300 free doughnuts to lucky winners. Sign up, follow us on social, share for bonus entries. Non-winners get a BOGO coupon."
        },
        {
          "subject": "Over 500 people already entered — have you?",
          "body": "The Great Doughnut Drop is blowing up. 500+ entries. Only a few days left. Don't miss your chance. Enter now — takes 10 seconds."
        }
      ],
      "seo": {
        "title": "Cowboy Tony's Doughnuts | Handcrafted Fresh Daily",
        "metaDescription": "Cowboy Tony's Doughnuts — handcrafted, fresh daily with a cowboy twist. Follow us for giveaways, new flavors, and limited drops.",
        "keywords": ["cowboy tonys doughnuts", "doughnuts near me", "fresh doughnuts", "best doughnuts", "doughnut giveaway", "artisan doughnuts"],
        "h1": "Handcrafted Doughnuts with a Cowboy Twist",
        "sections": [
          {"heading": "Our Story", "content": "Bold flavors, handcrafted quality, made fresh daily with premium ingredients."},
          {"heading": "The Great Doughnut Drop", "content": "Enter our 300-doughnut giveaway! Sign up for a chance to win. Non-winners get a BOGO coupon."},
          {"heading": "Catering & Events", "content": "Custom doughnut boxes for parties, office events, and weddings. Contact us for a quote."}
        ]
      },
      "landingPage": {
        "hero": "300 Free Doughnuts. One Lucky Signup Away.",
        "subheadline": "Enter The Great Doughnut Drop from Cowboy Tony's. Sign up, follow, win.",
        "cta": "Enter the Giveaway",
        "testimonials": [
          {"quote": "Best doughnuts I've ever had. The Maple Bourbon Pecan changed my life.", "author": "Sarah M.", "role": "Loyal Customer"},
          {"quote": "I drive 30 minutes just for the weekend drops.", "author": "Marcus T.", "role": "Weekend Regular"},
          {"quote": "Ordered 5 dozen for our office party. Gone in 20 minutes.", "author": "Kim L.", "role": "Corporate Client"}
        ],
        "pricing": [
          {"name": "Free Entry", "price": "$0", "features": ["Chance to win", "BOGO for non-winners"]},
          {"name": "Bonus Entry", "price": "$0", "features": ["Share on social", "Tag a friend", "Post a TikTok"]},
          {"name": "Top Engager", "price": "$0", "features": ["Win a half-dozen box", "Featured on our page"]}
        ],
        "faq": [
          {"question": "How do I enter?", "answer": "Sign up with your email and follow us on at least one platform."},
          {"question": "What if I don't win?", "answer": "Every non-winner gets a BOGO coupon redeemable in-store."},
          {"question": "When are winners announced?", "answer": "After the campaign window closes, via email and social media."}
        ],
        "seoMetadata": {"title": "The Great Doughnut Drop | 300 Free Doughnuts", "description": "Enter for a chance to win free doughnuts. 300 winners. BOGO for everyone else."}
      },
      "gbpPosts": [
        "🍩 The Great Doughnut Drop is LIVE! 300 free doughnuts — sign up on our website. Non-winners get BOGO!",
        "New flavor drop this week! Fresh daily. Follow us on social for first access.",
        "Catering available! Custom doughnut boxes for parties and events. DM or call for a quote! 🤠"
      ],
      "redditLaunch": "Cowboy Tony's is giving away 300 FREE doughnuts — The Great Doughnut Drop is live! Sign up with your email, follow on social. Non-winners get BOGO. Handcrafted fresh daily — Maple Bourbon Pecan, Strawberry Lemonade Glaze, and more. 🤠🍩",
      "discordAnnouncement": "🍩 **The Great Doughnut Drop** — Cowboy Tony's is giving away 300 FREE doughnuts! Sign up, follow us, and you could win. Non-winners get a BOGO coupon. Enter now!"
    }
    """;
}
