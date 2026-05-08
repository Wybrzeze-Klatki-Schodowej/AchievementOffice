using AchievementOffice.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AchievementOffice.Data.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRole");

            builder.HasKey(userRole => userRole.Id);

            builder.Property(userRole => userRole.Id)
                .HasColumnName("user_role_id");

            builder.Property(userRole => userRole.Name)
                .HasColumnName("user_role_name")
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}
