using AchievementOffice.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AchievementOffice.Data.Configurations;

public class UserDetailsConfiguration : IEntityTypeConfiguration<UserDetails>
{
    public void Configure(EntityTypeBuilder<UserDetails> builder)
    {
        builder.ToTable("UserDetails");

        builder.HasKey(userD => userD.UserId);

        builder.HasOne(userD => userD.User)
            .WithOne(user => user.UserDetails)
            .HasForeignKey<UserDetails>(userD => userD.UserId)
            .HasConstraintName("fk_user_details_user")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(userD => userD.UserId).HasColumnName("user_id");

        builder.Property(userD => userD.Firstname)
            .HasColumnName("firstname")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(userD => userD.Lastname)
            .HasColumnName("lastname")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(userD => userD.JobTitle)
            .HasColumnName("job_title")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(userD => userD.Bio)
            .HasColumnName("profile_bio");

        builder.Property(userD => userD.AvatarUrl)
            .HasColumnName("avatar_url")
            .HasMaxLength(255);

        builder.Property(ud => ud.VisibilityId)
            .HasColumnName("visibility_id")
            .HasDefaultValue(1) // Public
            .IsRequired();

        builder.HasOne(ud => ud.Visibility)
            .WithMany(v => v.UserDetailsList)
            .HasForeignKey(ud => ud.VisibilityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}