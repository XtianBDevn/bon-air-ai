using BonAirCampaigns.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace BonAirCampaigns.Api.Data.Migrations;

[DbContext(typeof(AppDbContext))]
partial class AppDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "9.0.0");
        modelBuilder.Entity("BonAirCampaigns.Api.Entities.Organization", b =>
        {
            b.Property<Guid>("Id").HasColumnType("uuid");
            b.HasKey("Id");
            b.ToTable("Organizations");
        });
    }
}
