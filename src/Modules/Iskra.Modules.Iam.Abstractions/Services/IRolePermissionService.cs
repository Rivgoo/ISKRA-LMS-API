using Iskra.Application.Results;

namespace Iskra.Modules.Iam.Abstractions.Services;

/// <summary>
/// Manages the definitions of what a Role is allowed to do.
/// </summary>
public interface IRolePermissionService
{
    /// <summary>
    /// Updates the permissions for a specific role.
    /// Replaces the entire current set with the new set.
    /// </summary>
    Task<Result> UpdateRolePermissionsAsync(Guid roleId, IEnumerable<string> permissions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current permissions assigned to a role.
    /// </summary>
    Task<Result<HashSet<string>>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available system permissions defined in code.
    /// </summary>
    Result<IEnumerable<string>> GetAllSystemPermissions();
}