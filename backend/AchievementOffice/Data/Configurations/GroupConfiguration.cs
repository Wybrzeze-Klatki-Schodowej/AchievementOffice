using AchievementOffice.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AchievementOffice.Data.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable("Groups");

            builder.HasKey(g => g.GroupId);

            builder.Property(g => g.GroupId)
                .HasColumnName("group_id");

            builder.Property(g => g.Name)
                .HasColumnName("group_name")
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(g => g.Description)
                .HasColumnName("group_description")
                .IsRequired();

            builder.Property(g => g.MaxUserCount)
                .HasColumnName("max_user_count")
                .IsRequired();

            builder.Property(g => g.AvatarUrl)
                .HasColumnName("group_avatar_url")
                .HasMaxLength(2000);

            builder.Property(g => g.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            builder.Property(g => g.UpdatedAt)
                .HasColumnName("updated_at")
                .IsRequired();

            builder.Property(g => g.DeletedAt)
                .HasColumnName("deleted_at");

            builder.HasMany(g => g.GroupUsers)
                .WithOne(gu => gu.Group)
                .HasForeignKey(gu => gu.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
