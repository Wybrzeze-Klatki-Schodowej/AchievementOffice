using AchievementOffice.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AchievementOffice.Data.Configurations;

public class RankingConfigurationMapping : IEntityTypeConfiguration<RankingConfiguration>
{
    public void Configure(EntityTypeBuilder<RankingConfiguration> builder)
    {
        builder.ToTable("RankingConfiguration", table =>
        {
            table.HasCheckConstraint("CK_RankingConfiguration_Singleton", "\"Id\" = 1");
            table.HasCheckConstraint("CK_RankingConfiguration_Points", "\"ShoutoutBase\" >= 0 AND \"ReactionShoutOutPoints\" >= 0 AND \"AchievementApprovedBasePoints\" >= 0 AND \"DisapprovalPoints\" <= 0");
        });
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();
        builder.Property(c => c.ShoutoutBase).HasPrecision(18, 2);
        builder.Property(c => c.ReactionShoutOutPoints).HasPrecision(18, 2);
        builder.Property(c => c.AchievementApprovedBasePoints).HasPrecision(18, 2);
        builder.Property(c => c.DisapprovalPoints).HasPrecision(18, 2);
        builder.HasData(new RankingConfiguration
        {
            Id = 1, ShoutoutBase = 10m, ReactionShoutOutPoints = 2m,
            AchievementApprovedBasePoints = 50m, DisapprovalPoints = -20m
        });
    }
}
