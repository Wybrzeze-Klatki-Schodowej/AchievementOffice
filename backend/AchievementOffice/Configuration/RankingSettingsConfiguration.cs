namespace AchievementOffice.Configuration
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
