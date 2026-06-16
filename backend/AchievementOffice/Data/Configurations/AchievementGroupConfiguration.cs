using AchievementOffice.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AchievementOffice.Data.Configurations;

public class AchievementGroupConfiguration : IEntityTypeConfiguration<AchievementGroup>
{
    public void Configure(EntityTypeBuilder<AchievementGroup> builder)
    {
        builder.ToTable("AchievementGroups");

        builder.HasKey(ag => new { ag.AchievementId, ag.GroupId });

        builder.Property(ag => ag.AchievementId).HasColumnName("achievement_id");
        builder.Property(ag => ag.GroupId).HasColumnName("group_id");

        builder.HasOne(ag => ag.Achievement)
            .WithMany(a => a.AchievementGroups)
            .HasForeignKey(ag => ag.AchievementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ag => ag.Group)
            .WithMany()
            .HasForeignKey(ag => ag.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}