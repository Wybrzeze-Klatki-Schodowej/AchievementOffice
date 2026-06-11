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
    public DbSet<GroupUser> GroupUsers => Set<GroupUser>();
    public DbSet<GroupUserRole> GroupUserRoles => Set<GroupUserRole>();
    public DbSet<Rank> Ranks => Set<Rank>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AchievementVerificationRequest> AchievementVerificationRequests 
        => Set<AchievementVerificationRequest>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}