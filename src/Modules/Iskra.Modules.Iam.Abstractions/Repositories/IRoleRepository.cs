using Iskra.Application.Abstractions.Repositories;
using Iskra.Core.Domain.Entities;
using Iskra.Modules.Iam.Abstractions.Models;

namespace Iskra.Modules.Iam.Abstractions.Repositories;

/// <summary>
/// Defines the contract for a repository that manages Role entities.
/// </summary>
public interface IRoleRepository : IEntityOperations<Role, Guid>
{
    /// <summary>
    /// Asynchronously retrieves a role by its unique name.
    /// </summary>
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all roles with the count of assigned users.
    /// Optimized projection query.
    /// </summary>
    Task<List<RoleDto>> GetAllWithCountsAsync(CancellationToken ct = default);
}