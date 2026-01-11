using System.IdentityModel.Tokens.Jwt;
using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using SmartCommune.Application.Common.Interfaces.Authentication;
using SmartCommune.Application.Common.Interfaces.Services;
using SmartCommune.Infrastructure.Authentication;
using SmartCommune.Infrastructure.Persistence;
using SmartCommune.Infrastructure.Services;

namespace SmartCommune.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        // Add settings.
        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(JwtSettings.SectionName))
            .ValidateOnStart(); // Kiểm tra ngay lúc app chạy xem file config có thiếu không.

        // Application services.
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        services
            .AddPersistence(configuration) // Db services.
            .AddAuth(configuration); // Auth services.

        return services;
    }

    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
        {
            string? connectionString = configuration.GetConnectionString("MySql");

            ArgumentException.ThrowIfNullOrEmpty(connectionString, "ConnectionStrings:MySql");

            optionsBuilder
                .UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString));
        });

        return services;
    }

    public static IServiceCollection AddAuth(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        // Lấy lại config đã bind ở trên hoặc lấy trực tiếp (nhưng phải đảm bảo không null).
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);

        // Nếu thiếu Secret là báo lỗi ngay lúc khởi động app, không đợi user login mới lỗi.
        if (string.IsNullOrEmpty(jwtSettings.Secret))
        {
            throw new InvalidOperationException("JwtSettings:Secret is missing in appsettings.json");
        }

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                ClockSkew = TimeSpan.FromSeconds(3),
                NameClaimType = JwtRegisteredClaimNames.Sub,
            };
        });

        return services;
    }
}