using Iskra.Application.Abstractions.Repositories;
using Iskra.Core.Domain.Entities;

namespace Iskra.Modules.Users.Abstractions.Repositories;

/// <summary>
/// Defines the contract for a repository that manages Role entities.
/// </summary>
public interface IRoleRepository : IEntityOperations<Role, Guid>
{
    /// <summary>
    /// Asynchronously retrieves a role by its unique name.
    /// </summary>
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}