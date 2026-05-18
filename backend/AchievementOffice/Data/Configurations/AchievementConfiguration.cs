using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AchievementOffice.Data.Configurations;

public class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
{
    public void Configure(EntityTypeBuilder<Achievement> builder)
    {
        builder.ToTable("Achievements");

        builder.HasKey(a => a.AchievementId);

        builder.Property(a => a.AchievementId)
            .HasColumnName("achievement_id");

        builder.Property(a => a.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(a => a.Title)
            .HasColumnName("title")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(a => a.DeletedAt)
            .HasColumnName("deleted_at");
    }
}