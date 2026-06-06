using AchievementOffice.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AchievementOffice.Data.Configurations
{
    public class GroupUserRoleConfiguration : IEntityTypeConfiguration<GroupUserRole>
    {
        public void Configure(EntityTypeBuilder<GroupUserRole> builder)
        {
            builder.ToTable("GroupUserRole");

            builder.HasKey(gur => gur.GroupUserRoleId);

            builder.Property(gur => gur.GroupUserRoleId)
                .HasColumnName("group_user_role_id");

            builder.Property(gur => gur.Name)
                .HasColumnName("group_user_role_name")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(gur => gur.IsAdmin)
                .HasColumnName("is_admin")
                .IsRequired();

            builder.Property(gur => gur.GroupId)
                .HasColumnName("group_id")
                .IsRequired();

            builder.HasOne(gur => gur.Group)
                .WithMany(g => g.UserRoles)
                .HasForeignKey(gur => gur.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
