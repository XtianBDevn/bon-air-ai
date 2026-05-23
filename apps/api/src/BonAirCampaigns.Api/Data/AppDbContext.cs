using BonAirCampaigns.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace BonAirCampaigns.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<CampaignAsset> CampaignAssets => Set<CampaignAsset>();
    public DbSet<PromptTemplate> PromptTemplates => Set<PromptTemplate>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<UsageLog> UsageLogs => Set<UsageLog>();
    public DbSet<BrandKit> BrandKits => Set<BrandKit>();
    public DbSet<ExportRecord> Exports => Set<ExportRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>(e =>
        {
            e.HasIndex(x => x.Slug).IsUnique();
            e.HasIndex(x => x.StripeCustomerId);
        });

        modelBuilder.Entity<AppUser>(e =>
        {
            e.HasIndex(x => x.SupabaseUserId).IsUnique();
            e.HasIndex(x => x.Email);
            e.HasOne(x => x.Organization).WithMany(o => o.Users).HasForeignKey(x => x.OrganizationId);
        });

        modelBuilder.Entity<Campaign>(e =>
        {
            e.HasOne(x => x.Organization).WithMany(o => o.Campaigns).HasForeignKey(x => x.OrganizationId);
            e.HasOne(x => x.CreatedBy).WithMany().HasForeignKey(x => x.CreatedByUserId);
            e.HasIndex(x => new { x.OrganizationId, x.Status });
        });

        modelBuilder.Entity<CampaignAsset>(e =>
        {
            e.HasOne(x => x.Campaign).WithMany(c => c.Assets).HasForeignKey(x => x.CampaignId);
        });

        modelBuilder.Entity<BrandKit>(e =>
        {
            e.HasOne(x => x.Organization).WithOne(o => o.BrandKit).HasForeignKey<BrandKit>(x => x.OrganizationId);
        });

        modelBuilder.Entity<PromptTemplate>(e =>
        {
            e.HasIndex(x => x.AgentKey);
        });

        modelBuilder.Entity<UsageLog>(e =>
        {
            e.HasIndex(x => new { x.OrganizationId, x.CreatedAt });
        });
    }
}
