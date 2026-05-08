using AchievementOffice.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AchievementOffice.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserRole> UserRoles { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
