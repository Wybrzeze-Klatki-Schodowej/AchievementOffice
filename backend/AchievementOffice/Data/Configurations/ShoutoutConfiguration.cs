using AchievementOffice.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AchievementOffice.Data.Configurations;

public class ShoutoutConfiguration : IEntityTypeConfiguration<Shoutout>
{
    public void Configure(EntityTypeBuilder<Shoutout> builder)
    {
        builder.ToTable("Shoutouts");

        builder.HasKey(s => s.ShoutoutId);

        builder.Property(s => s.ShoutoutId)
            .HasColumnName("shoutout_id");

        builder.Property(s => s.SenderId)
            .HasColumnName("sender_id")
            .IsRequired();

        builder.Property(s => s.ReceiverId)
            .HasColumnName("receiver_id")
            .IsRequired();

        builder.Property(s => s.Title)
            .HasColumnName("title")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(s => s.DeletedAt)
            .HasColumnName("deleted_at");
    }
}