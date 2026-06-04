using AchievementOffice.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AchievementOffice.Data;

public class AppDbContext : DbContext
{
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserDetails> UserDetails => Set<UserDetails>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<AchievementApprove> AchievementApproves => Set<AchievementApprove>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}