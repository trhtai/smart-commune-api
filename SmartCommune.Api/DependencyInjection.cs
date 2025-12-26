using Microsoft.Extensions.Options;
using SmartCommune.Api.Configurations;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SmartCommune.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(
        this IServiceCollection services,
        IWebHostEnvironment env)
    {
        // SWAGGER
        if (env.IsDevelopment())
        {
            services.AddSwaggerGen();
            services.AddTransient<
                IConfigureOptions<SwaggerGenOptions>,
                ConfigureSwaggerOptions>();
        }

        // CONTROLLERS
        services.AddControllers();

        return services;
    }
}
