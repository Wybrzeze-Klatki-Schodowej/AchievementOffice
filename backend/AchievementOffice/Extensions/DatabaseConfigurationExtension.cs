using AchievementOffice.Configuration;
using AchievementOffice.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AchievementOffice.Extensions;

public static class DatabaseConfigurationExtension 
{
    public static IServiceCollection SetupDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var dbSettings = configuration.GetSection(DatabaseConfiguration.SectionName).Get<DatabaseConfiguration>();

        if (dbSettings == null) 
            throw new InvalidOperationException($"{DatabaseConfiguration.SectionName} is missing from appsettings");

        var connectionString = BuildConnectionString(dbSettings);

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        return services;
    }

    private static string BuildConnectionString(DatabaseConfiguration dbConfiguration)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Port = dbConfiguration.Port,
            Host = dbConfiguration.Host,
            Database = dbConfiguration.Database,
            Username = dbConfiguration.Username,
            Password = dbConfiguration.Password
        };

        return connectionStringBuilder.ConnectionString;
    }
}