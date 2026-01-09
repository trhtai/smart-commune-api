using Microsoft.Extensions.DependencyInjection;

namespace SmartCommune.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        return services;
    }
}