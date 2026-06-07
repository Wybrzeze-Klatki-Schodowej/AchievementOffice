using AchievementOffice.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AchievementOffice.Data;

public class AppDbContext : DbContext
{
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserDetails> UserDetails => Set<UserDetails>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<AchievementApprove> AchievementApproves => Set<AchievementApprove>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<GroupUser> GroupUser => Set<GroupUser>();
    public DbSet<GroupUserRole> GroupUserRoles => Set<GroupUserRole>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Shoutout> Shoutouts => Set<Shoutout>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}