using Iskra.Application.Abstractions.Repositories;
using Iskra.Infrastructure.Shared.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Iskra.Infrastructure.Shared;

public static class SharedInfrastructureInjection
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}