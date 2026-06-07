using AchievementOffice.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AchievementOffice.Data.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("comment_id")
            .HasDefaultValueSql("gen_random_uuid()");

        builder.Property(c => c.AuthorId)
            .HasColumnName("author_id")
            .IsRequired();

        builder.Property(c => c.ProfileUserId)
            .HasColumnName("profile_user_id")
            .IsRequired();

        builder.Property(c => c.Content)
            .HasColumnName("content")
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.Property(c => c.DeletedAt)
            .HasColumnName("deleted_at");

        builder.HasOne(c => c.Author)
            .WithMany()
            .HasForeignKey(c => c.AuthorId)
            .HasConstraintName("fk_comments_author")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.ProfileUser)
            .WithMany()
            .HasForeignKey(c => c.ProfileUserId)
            .HasConstraintName("fk_comments_profile_user")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(c => c.ProfileUserId)
            .HasDatabaseName("ix_comments_profile_user");

        builder.HasIndex(c => c.AuthorId)
            .HasDatabaseName("ix_comments_author");
    }
}