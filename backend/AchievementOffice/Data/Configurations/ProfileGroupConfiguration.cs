using AchievementOffice.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AchievementOffice.Data.Configurations;

public class ProfileGroupConfiguration : IEntityTypeConfiguration<ProfileGroup>
{
    public void Configure(EntityTypeBuilder<ProfileGroup> builder)
    {
        builder.ToTable("ProfileGroups");

        builder.HasKey(pg => new { pg.UserId, pg.GroupId });

        builder.Property(pg => pg.UserId).HasColumnName("user_id");
        builder.Property(pg => pg.GroupId).HasColumnName("group_id");

        builder.HasOne(pg => pg.UserDetails)
            .WithMany(ud => ud.ProfileGroups)
            .HasForeignKey(pg => pg.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pg => pg.Group)
            .WithMany()
            .HasForeignKey(pg => pg.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}