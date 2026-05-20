using AchievementOffice.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AchievementOffice.Extensions;

public static class SecurityConfigurationExtension
{
    public static IServiceCollection AddSecurityConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var corsConfiguration = configuration.GetSection(CorsConfiguration.SectionName).Get<CorsConfiguration>();
        var jwtConfiguration = configuration.GetSection(JwtConfiguration.SectionName).Get<JwtConfiguration>();

        if (corsConfiguration == null) throw new InvalidOperationException("CORS configuration is missing.");
        if (jwtConfiguration == null) throw new InvalidOperationException("JWT configuration is missing.");

        services.AddCors(options => AddReactPolicy(options, corsConfiguration.FrontUrl));

        services.AddAuthentication(opt =>
        {
<<<<<<< HEAD
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
=======
            var corsConfiguration = configuration.GetSection(CorsConfiguration.SectionName).Get<CorsConfiguration>();
            var jwtConfiguration = configuration.GetSection(JwtConfiguration.SectionName).Get<JwtConfiguration>();

            if (corsConfiguration == null) throw new InvalidOperationException("CORS configuration is missing.");
            if (jwtConfiguration == null) throw new InvalidOperationException("JWT configuration is missing.");

            Console.WriteLine("JWT ISSUER: " + jwtConfiguration.Issuer);
Console.WriteLine("JWT AUDIENCE: " + jwtConfiguration.Audience);
Console.WriteLine("JWT KEY: " + jwtConfiguration.SigningKey);

            services.AddCors(options => AddReactPolicy(options, corsConfiguration.FrontUrl));

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = GetValidationParameters(jwtConfiguration!);
                options.Events = GetJwtBearerEvents();
            });

            return services;
        }

        private static void AddReactPolicy(CorsOptions options, string frontUrl)
>>>>>>> d4df33e ([AOF-18] Added basic possibility to manage shoutouts from the frontend)
        {
            options.TokenValidationParameters = GetValidationParameters(jwtConfiguration!);
            options.Events = GetJwtBearerEvents();
        });

        return services;
    }

    private static void AddReactPolicy(CorsOptions options, string frontUrl)
    {
        options.AddPolicy("ReactPolicy", policy =>
        {
            policy.WithOrigins(frontUrl)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    }

    private static TokenValidationParameters GetValidationParameters(JwtConfiguration jwtConfiguration)
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfiguration.Issuer,
            ValidAudience = jwtConfiguration.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SigningKey))
        };
    }

    private static JwtBearerEvents GetJwtBearerEvents()
    {
        return new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("X-jwt-token"))
                {
                    context.Token = context.Request.Cookies["X-jwt-token"];
                }

                return Task.CompletedTask;
            }
        };
    }
}