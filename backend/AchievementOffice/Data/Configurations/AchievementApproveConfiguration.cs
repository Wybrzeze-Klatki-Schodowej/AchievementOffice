using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AchievementOffice.Features.Achievements;

public class AchievementApproveConfiguration : IEntityTypeConfiguration<AchievementApprove>
{
    public void Configure(EntityTypeBuilder<Achievement> builder)
    {
        builder.ToTable("AchievementApprove");

        builder.HasKey(a => a.AchievementApproveId);

        builder.Property(a => a.AchievementApproveId)
            .HasColumnName("achievement_approve_id");

        builder.Property(a => a.AchievementId )
            .HasColumnName("achievement_id")
            .IsRequired();

        builder.Property(a => a.UserId )
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(a => a.IsApproved )
            .HasColumnName("is_approved")
            .IsRequired();

        builder.Property(a => a.ApprovedAt )
            .HasColumnName("approved_at")
            .IsRequired();

        builder.Property(a => a.DeletedAt )
            .HasColumnName("deleted_at");

        builder.HasIndex(a => new { a.AchievementId, a.UserId })
            .IsUnique();
    }
}