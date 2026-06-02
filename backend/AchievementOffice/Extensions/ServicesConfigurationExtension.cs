using AchievementOffice.Services;

namespace AchievementOffice.Extensions;

public static class ServicesConfigurationExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<IUserService, UserService>();

        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IAchievementService, AchievementService>();

        services.AddScoped<IAdminService, AdminService>();

        return services;
    }
}