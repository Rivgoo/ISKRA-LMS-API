using Iskra.Application.Abstractions.Repositories;

namespace Iskra.Modules.Iam.Abstractions.Repositories;

public interface IPermissionRepository : IRepository
{
    /// <summary>
    /// Retrieves all permission strings assigned to a specific role.
    /// </summary>
    Task<HashSet<string>> GetPermissionsForRoleAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all existing permissions for a role and adds the new set.
    /// </summary>
    Task ReplacePermissionsAsync(Guid roleId, IEnumerable<string> newPermissions, CancellationToken cancellationToken = default);
}