using AchievementOffice.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AchievementOffice.Data.Configurations;

public class VisibilityConfiguration : IEntityTypeConfiguration<Visibility>
{
    public void Configure(EntityTypeBuilder<Visibility> builder)
    {
        builder.ToTable("Visibility");

        builder.HasKey(v => v.VisibilityModeId);

        builder.Property(v => v.VisibilityModeId)
            .HasColumnName("visibility_mode_id");

        builder.Property(v => v.VisibilityModeName)
            .HasColumnName("visibility_mode_name")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasData(
            new Visibility { VisibilityModeId = 1, VisibilityModeName = "Public" },
            new Visibility { VisibilityModeId = 2, VisibilityModeName = "Private" },
            new Visibility { VisibilityModeId = 3, VisibilityModeName = "Group" }
        );
    }
}