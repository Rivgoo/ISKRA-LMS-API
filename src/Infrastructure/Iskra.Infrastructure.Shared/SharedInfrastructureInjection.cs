using Iskra.Application.Abstractions.Caching;
using Iskra.Application.Abstractions.Repositories;
using Iskra.Application.Abstractions.Security;
using Iskra.Infrastructure.Shared.Caching;
using Iskra.Infrastructure.Shared.Persistence;
using Iskra.Infrastructure.Shared.Security;
using Microsoft.Extensions.DependencyInjection;

namespace Iskra.Infrastructure.Shared;

public static class SharedInfrastructureInjection
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
    {
        // Database
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Security
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // Caching
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, MemoryCacheService>();

        return services;
    }
}