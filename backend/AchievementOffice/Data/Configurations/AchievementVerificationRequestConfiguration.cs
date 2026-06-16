using AchievementOffice.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AchievementOffice.Data.Configurations;

public class AchievementVerificationRequestConfiguration
    : IEntityTypeConfiguration<AchievementVerificationRequest>
{
    public void Configure(
        EntityTypeBuilder<AchievementVerificationRequest> builder)
    {
        builder.ToTable("achievement_verification_requests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.AchievementId)
            .HasColumnName("achievement_id")
            .IsRequired();

        builder.Property(x => x.RequesterUserId)
            .HasColumnName("requester_user_id")
            .IsRequired();

        builder.Property(x => x.TargetUserId)
            .HasColumnName("target_user_id")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.RespondedAt)
            .HasColumnName("responded_at");

        builder.HasOne(x => x.Achievement)
            .WithMany()
            .HasForeignKey(x => x.AchievementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.RequesterUser)
            .WithMany()
            .HasForeignKey(x => x.RequesterUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.TargetUser)
            .WithMany()
            .HasForeignKey(x => x.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}