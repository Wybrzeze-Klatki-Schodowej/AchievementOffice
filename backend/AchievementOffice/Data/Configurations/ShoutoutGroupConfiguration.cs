using AchievementOffice.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AchievementOffice.Data.Configurations;

public class ShoutoutGroupConfiguration : IEntityTypeConfiguration<ShoutoutGroup>
{
    public void Configure(EntityTypeBuilder<ShoutoutGroup> builder)
    {
        builder.ToTable("ShoutoutGroups");

        builder.HasKey(sg => new { sg.ShoutoutId, sg.GroupId });

        builder.Property(sg => sg.ShoutoutId).HasColumnName("shoutout_id");
        builder.Property(sg => sg.GroupId).HasColumnName("group_id");

        builder.HasOne(sg => sg.Shoutout)
            .WithMany(s => s.ShoutoutGroups)
            .HasForeignKey(sg => sg.ShoutoutId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sg => sg.Group)
            .WithMany()
            .HasForeignKey(sg => sg.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}