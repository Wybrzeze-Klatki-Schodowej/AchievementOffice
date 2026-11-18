using AchievementOffice.Data;
using AchievementOffice.Entities;
using Microsoft.EntityFrameworkCore;

namespace AchievementOffice.Extensions;

public static class DevelopmentAdminInitializationExtension
{
    public static async Task InitializeDevelopmentAdminAsync(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment() || !app.Configuration.GetValue<bool>("DevelopmentAdmin:Enabled"))
            return;

        var username = app.Configuration["DevelopmentAdmin:Username"];
        var email = app.Configuration["DevelopmentAdmin:Email"];
        var password = app.Configuration["DevelopmentAdmin:Password"];

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            return;

        await using var scope = app.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (await context.Users.AnyAsync(u => u.Login == username || u.Email == email))
            return;

        var adminRole = await context.UserRoles.SingleOrDefaultAsync(r => r.Name == "Admin") ?? throw new InvalidOperationException("Admin role is missing");

        var now = DateTime.UtcNow;
        context.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Login = username,
            Email = email,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            UserRoleId = adminRole.Id,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now,
            UserDetails = new UserDetails
            {
                Firstname = "Local",
                Lastname = "Admin",
                JobTitle = "Development administrator"
            }
        });

        await context.SaveChangesAsync();
    }
}
