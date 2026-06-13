using AchievementOffice.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AchievementOffice.Data.Configurations;

public class KudosShoutoutConfiguration : IEntityTypeConfiguration<KudosShoutout>
{
    public void Configure(EntityTypeBuilder<KudosShoutout> builder)
    {
        builder.ToTable("KudosShoutouts");

        builder.HasKey(ks => new { ks.ShoutoutId, ks.UserId });

        builder.Property(ks => ks.ShoutoutId)
            .HasColumnName("shoutout_id");

        builder.Property(ks => ks.UserId)
            .HasColumnName("user_id");

        builder.Property(ks => ks.Reaction)
            .HasColumnName("reaction")
            .IsRequired();

        builder.HasOne(ks => ks.Shoutout)
            .WithMany(s => s.Kudos)
            .HasForeignKey(ks => ks.ShoutoutId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ks => ks.User)
            .WithMany(u => u.ShoutoutReactions)
            .HasForeignKey(ks => ks.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
