using AchievementOffice.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AchievementOffice.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(user => user.Id);

            builder.Property(user => user.Id)
                .HasColumnName("user_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(user => user.Login)
                .HasColumnName("login")
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(user => user.Login)
                .IsUnique()
                .HasDatabaseName("ix_user_login");

            builder.Property(user => user.Password)
                .HasColumnName("password_hash")
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(user => user.LastPassword)
                .HasColumnName("last_password_hash")
                .HasMaxLength(255);

            builder.Property(user => user.Email)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(user => user.Email)
                .IsUnique()
                .HasDatabaseName("ix_user_email");

            builder.Property(user => user.LastEmail)
                .HasColumnName("last_email")
                .HasMaxLength(100);

            builder.Property(user => user.UserRoleId)
                .HasColumnName("user_role_id")
                .IsRequired();

            builder.HasOne(u => u.UserRole)
                .WithMany(ur => ur.Users)
                .HasForeignKey(u => u.UserRoleId)
                .HasConstraintName("fk_users_roles")
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(u => u.IsActive)
                .HasColumnName("is_active");

            builder.Property(u => u.DeletedAt)
                .HasColumnName("deleted_at");

            builder.Property(u => u.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(u => u.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasData(
                new User
                {
                    Id = Guid.Parse("a5e2f6d1-4b7c-4d8e-9f0a-1b2c3d4e5f6f"),
                    Login = "admin_test",
                    Email = "admin@example.com",
                    Password = "nihjcbweiij12",
                    UserRoleId = Guid.Parse("fb279f32-7235-4306-8968-380f76953e6b"),
                    CreatedAt = new DateTime(2026, 05, 10, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 05, 10, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
