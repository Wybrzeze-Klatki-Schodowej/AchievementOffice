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
                .HasColumnName("user_role_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(userRole => userRole.Name)
                .HasColumnName("user_role_name")
                .HasMaxLength(50)
                .IsRequired();

            var adminId = Guid.Parse("fb279f32-7235-4306-8968-380f76953e6b");
            var userId = Guid.Parse("7a610998-3843-4315-9923-92f7634f1981");

            builder.HasData(
                new UserRole { Id = adminId, Name = "Admin" },
                new UserRole { Id = userId, Name = "User" }
            );
        }
    }
}
