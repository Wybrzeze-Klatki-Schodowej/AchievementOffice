using AchievementOffice.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AchievementOffice.Data.Configurations
{
    public class GroupUserConfiguration : IEntityTypeConfiguration<GroupUser>
    {
        public void Configure(EntityTypeBuilder<GroupUser> builder)
        {
            builder.ToTable("GroupUser");

            builder.HasKey(gu => new { gu.GroupId, gu.UserId });

            builder.Property(gu => gu.GroupId)
                .HasColumnName("group_id")
                .IsRequired();

            builder.Property(gu => gu.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(gu => gu.GroupUserRoleId)
                .HasColumnName("group_user_role_id")
                .IsRequired();

            builder.HasOne(gu => gu.Group)
                .WithMany(g => g.GroupUsers)
                .HasForeignKey(gu => gu.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(gu => gu.User)
                .WithMany(u => u.GroupUsers)
                .HasForeignKey(gu => gu.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(gu => gu.GroupUserRoleId)
                .WithMany()
                .HasForeignKey(gu => gu.GroupUserRoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
