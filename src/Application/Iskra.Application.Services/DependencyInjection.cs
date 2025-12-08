using Iskra.Application.Abstractions.Services.Entities;
using Iskra.Application.Services.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Iskra.Application.Services;

/// <summary>
/// Handles the registration of the Core Application Services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the application layer services (Business Logic).
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserEntityService, UserEntityService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IRolePermissionService, RolePermissionService>();

        return services;
    }
}