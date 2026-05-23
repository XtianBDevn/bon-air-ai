using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BonAirCampaigns.Api.Data.Migrations;

public partial class Initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Organizations",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                Slug = table.Column<string>(type: "text", nullable: false),
                LogoUrl = table.Column<string>(type: "text", nullable: true),
                Plan = table.Column<string>(type: "text", nullable: false),
                StripeCustomerId = table.Column<string>(type: "text", nullable: true),
                StripeSubscriptionId = table.Column<string>(type: "text", nullable: true),
                SubscriptionStatus = table.Column<string>(type: "text", nullable: false),
                CampaignsUsedThisMonth = table.Column<int>(type: "integer", nullable: false),
                CampaignLimit = table.Column<int>(type: "integer", nullable: false),
                WhiteLabelEnabled = table.Column<bool>(type: "boolean", nullable: false),
                CustomDomain = table.Column<string>(type: "text", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Organizations", x => x.Id));

        migrationBuilder.CreateTable(
            name: "PromptTemplates",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                AgentKey = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                SystemPrompt = table.Column<string>(type: "text", nullable: false),
                UserPromptTemplate = table.Column<string>(type: "text", nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                Version = table.Column<int>(type: "integer", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_PromptTemplates", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                SupabaseUserId = table.Column<string>(type: "text", nullable: false),
                Email = table.Column<string>(type: "text", nullable: false),
                FullName = table.Column<string>(type: "text", nullable: true),
                AvatarUrl = table.Column<string>(type: "text", nullable: true),
                Role = table.Column<string>(type: "text", nullable: false),
                IsPlatformAdmin = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                OrganizationId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
                table.ForeignKey(
                    name: "FK_Users_Organizations_OrganizationId",
                    column: x => x.OrganizationId,
                    principalTable: "Organizations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "BrandKits",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                LogoUrl = table.Column<string>(type: "text", nullable: true),
                PrimaryColor = table.Column<string>(type: "text", nullable: false),
                SecondaryColor = table.Column<string>(type: "text", nullable: false),
                AccentColor = table.Column<string>(type: "text", nullable: false),
                FontFamily = table.Column<string>(type: "text", nullable: false),
                Tone = table.Column<string>(type: "text", nullable: false),
                Tagline = table.Column<string>(type: "text", nullable: true),
                OpenAiApiKeyEncrypted = table.Column<string>(type: "text", nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BrandKits", x => x.Id);
                table.ForeignKey(
                    name: "FK_BrandKits_Organizations_OrganizationId",
                    column: x => x.OrganizationId,
                    principalTable: "Organizations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Subscriptions",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                StripeSubscriptionId = table.Column<string>(type: "text", nullable: false),
                StripePriceId = table.Column<string>(type: "text", nullable: false),
                Plan = table.Column<string>(type: "text", nullable: false),
                Status = table.Column<string>(type: "text", nullable: false),
                CurrentPeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Subscriptions", x => x.Id);
                table.ForeignKey(
                    name: "FK_Subscriptions_Organizations_OrganizationId",
                    column: x => x.OrganizationId,
                    principalTable: "Organizations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Campaigns",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                Status = table.Column<string>(type: "text", nullable: false),
                BusinessType = table.Column<string>(type: "text", nullable: false),
                Offer = table.Column<string>(type: "text", nullable: false),
                TargetAudience = table.Column<string>(type: "text", nullable: false),
                Location = table.Column<string>(type: "text", nullable: false),
                Tone = table.Column<string>(type: "text", nullable: false),
                Budget = table.Column<decimal>(type: "numeric", nullable: true),
                ChannelsJson = table.Column<string>(type: "text", nullable: false),
                GeneratedJson = table.Column<string>(type: "text", nullable: true),
                Headline = table.Column<string>(type: "text", nullable: true),
                Cta = table.Column<string>(type: "text", nullable: true),
                TokensUsed = table.Column<int>(type: "integer", nullable: false),
                ProgressPercent = table.Column<int>(type: "integer", nullable: false),
                CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Campaigns", x => x.Id);
                table.ForeignKey(
                    name: "FK_Campaigns_Organizations_OrganizationId",
                    column: x => x.OrganizationId,
                    principalTable: "Organizations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Campaigns_Users_CreatedByUserId",
                    column: x => x.CreatedByUserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "UsageLogs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: true),
                CampaignId = table.Column<Guid>(type: "uuid", nullable: true),
                EventType = table.Column<string>(type: "text", nullable: false),
                TokensUsed = table.Column<int>(type: "integer", nullable: false),
                MetadataJson = table.Column<string>(type: "text", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UsageLogs", x => x.Id);
                table.ForeignKey(
                    name: "FK_UsageLogs_Organizations_OrganizationId",
                    column: x => x.OrganizationId,
                    principalTable: "Organizations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CampaignAssets",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                AssetType = table.Column<string>(type: "text", nullable: false),
                Title = table.Column<string>(type: "text", nullable: false),
                Content = table.Column<string>(type: "text", nullable: false),
                MetadataJson = table.Column<string>(type: "text", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CampaignAssets", x => x.Id);
                table.ForeignKey(
                    name: "FK_CampaignAssets_Campaigns_CampaignId",
                    column: x => x.CampaignId,
                    principalTable: "Campaigns",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Exports",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                Format = table.Column<string>(type: "text", nullable: false),
                FileUrl = table.Column<string>(type: "text", nullable: true),
                Status = table.Column<string>(type: "text", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Exports", x => x.Id);
                table.ForeignKey(
                    name: "FK_Exports_Campaigns_CampaignId",
                    column: x => x.CampaignId,
                    principalTable: "Campaigns",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_Organizations_Slug", table: "Organizations", column: "Slug", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Users_SupabaseUserId", table: "Users", column: "SupabaseUserId", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Campaigns_OrganizationId_Status", table: "Campaigns", columns: new[] { "OrganizationId", "Status" });
        migrationBuilder.CreateIndex(name: "IX_PromptTemplates_AgentKey", table: "PromptTemplates", column: "AgentKey");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "CampaignAssets");
        migrationBuilder.DropTable(name: "Exports");
        migrationBuilder.DropTable(name: "UsageLogs");
        migrationBuilder.DropTable(name: "BrandKits");
        migrationBuilder.DropTable(name: "Subscriptions");
        migrationBuilder.DropTable(name: "PromptTemplates");
        migrationBuilder.DropTable(name: "Campaigns");
        migrationBuilder.DropTable(name: "Users");
        migrationBuilder.DropTable(name: "Organizations");
    }
}
