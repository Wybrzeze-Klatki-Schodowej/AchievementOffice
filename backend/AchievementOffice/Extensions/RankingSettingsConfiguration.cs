using AchievementOffice.Configuration;

namespace AchievementOffice.Extensions
{
    public static class RankingSettingsConfiguration
    {
        public static IServiceCollection AddRankingSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RankingSettings>(configuration.GetSection("RankingSettings"));
            return services;
        }
    }
}
