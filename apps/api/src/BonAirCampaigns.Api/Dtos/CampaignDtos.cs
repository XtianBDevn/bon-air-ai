namespace BonAirCampaigns.Api.Dtos;

public record CreateCampaignRequest(
    string Name,
    string BusinessType,
    string Offer,
    string TargetAudience,
    string Location,
    string Tone,
    decimal? Budget,
    List<string> Channels);

public record CampaignDto(
    Guid Id,
    string Name,
    string Status,
    string BusinessType,
    string Offer,
    string TargetAudience,
    string Location,
    string Tone,
    decimal? Budget,
    List<string> Channels,
    string? Headline,
    string? Cta,
    int TokensUsed,
    int ProgressPercent,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    object? Generated);

public record CampaignAssetDto(Guid Id, string AssetType, string Title, string Content, DateTime CreatedAt);

public record DashboardDto(
    int TotalCampaigns,
    int ActiveCampaigns,
    int CompletedCampaigns,
    int TokensUsedThisMonth,
    int CampaignsRemaining,
    string Plan,
    List<CampaignDto> RecentCampaigns,
    List<CampaignAssetDto> RecentAssets,
    List<UsageChartPoint> UsageChart);

public record UsageChartPoint(string Date, int Tokens);

public record UpdateBrandKitRequest(
    string? LogoUrl,
    string PrimaryColor,
    string SecondaryColor,
    string AccentColor,
    string FontFamily,
    string Tone,
    string? Tagline);

public record BrandKitDto(
    Guid Id,
    string? LogoUrl,
    string PrimaryColor,
    string SecondaryColor,
    string AccentColor,
    string FontFamily,
    string Tone,
    string? Tagline);

public record CheckoutRequest(string Plan, string SuccessUrl, string CancelUrl);

public record MeResponse(
    Guid Id,
    string Email,
    string? FullName,
    string Role,
    bool IsPlatformAdmin,
    OrganizationDto Organization);

public record OrganizationDto(
    Guid Id,
    string Name,
    string Slug,
    string Plan,
    int CampaignLimit,
    int CampaignsUsedThisMonth,
    string SubscriptionStatus,
    bool WhiteLabelEnabled);

public record AdminStatsDto(
    int TotalUsers,
    int TotalOrganizations,
    int TotalCampaigns,
    long TotalTokens,
    List<PlanBreakdown> Plans);

public record PlanBreakdown(string Plan, int Count);
