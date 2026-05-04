using AchievementOffice.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AchievementOffice.Extensions
{
    public static class DatabaseConfigurationExtension 
    {
        public static IServiceCollection SetupDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connnectionString = BuildConnectionString(configuration);

            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connnectionString));

            return services;
        }

        private static string BuildConnectionString(IConfiguration configuration)
        {
            var connectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Port = configuration.GetValue<int>("DatabaseSettings:Port", 5432),
                Host = configuration.GetValue<string>("DatabaseSettings:Host") ?? throw new ArgumentNullException("Host is missing"),
                Database = configuration.GetValue<string>("DatabaseSettings:Database") ?? throw new ArgumentNullException("Database is missing"),
                Username = configuration.GetValue<string>("DatabaseSettings:Username") ?? throw new ArgumentNullException("User is missing"),
                Password = configuration.GetValue<string>("DatabaseSettings:Password") ?? throw new ArgumentNullException("Password is missing")
            };

            return connectionStringBuilder.ConnectionString;
        }
    }
}
