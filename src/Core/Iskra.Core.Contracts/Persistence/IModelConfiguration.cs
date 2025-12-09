using Microsoft.EntityFrameworkCore;

namespace Iskra.Core.Contracts.Persistence;

/// <summary>
/// Implemented by Modules to register their entities with the EF Core ModelBuilder.
/// </summary>
public interface IModelConfiguration
{
    void Configure(ModelBuilder builder);
}