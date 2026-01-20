using System.Reflection;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using SmartCommune.Application.Common.Behaviors;
using SmartCommune.Application.Common.Mapping;
using SmartCommune.Application.Services.Identity.MenuItems;
using SmartCommune.Application.Services.Identity.Permissions;

namespace SmartCommune.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // Add MediatR.
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        // Add Validation Behaviors Pipeline.
        services.AddScoped(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // MAPPINGS.
        services.AddMappings();

        // SERVICES.
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IMenuItemService, MenuItemService>();

        return services;
    }
}