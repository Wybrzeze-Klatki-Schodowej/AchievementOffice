using AchievementOffice.Entities;
using Microsoft.EntityFrameworkCore;

namespace AchievementOffice.Data.Configurations
{
    public class RankConfiguration : IEntityTypeConfiguration<Rank>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Rank> builder)
        {
            builder.ToTable("Ranks");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .HasColumnName("rank_id")
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(r => r.Name)
                .HasColumnName("rank_name")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(r => r.Multiplier)
                .HasColumnName("multiplier")
                .HasDefaultValue(1m);
        }
    }
}
