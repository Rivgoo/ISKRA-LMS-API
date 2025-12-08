using Iskra.Application.Abstractions.Repositories;
using Iskra.Application.Abstractions.Repositories.Base;
using Iskra.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Iskra.Infrastructure.Persistence;

/// <summary>
/// Provides extension methods for registering persistence-layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds repositories and the Unit of Work to the service collection.
    /// </summary>
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();

        return services;
    }
}