using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AchievementOffice.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Achievement> Achievements => Set<Achievement>();
        public DbSet<AchievementApprove> AchievementApproves => Set<AchievementApprove>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
