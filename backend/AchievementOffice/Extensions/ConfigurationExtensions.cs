using AchievementOffice.Configuration;
using System.Runtime.CompilerServices;

namespace AchievementOffice.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection ConfigureAppConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<JwtConfiguration>()
                .Bind(configuration.GetSection(JwtConfiguration.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<CorsConfiguration>()
                .Bind(configuration.GetSection(CorsConfiguration.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<DatabaseConfiguration>()
                .Bind(configuration.GetSection(DatabaseConfiguration.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }
    }
}
