namespace Iskra.Modules.Iam.Abstractions.Services;

public interface IPermissionService
{
    /// <summary>
    /// Checks if the given roles imply the required permission.
    /// Uses caching internally.
    /// </summary>
    Task<bool> HasPermissionAsync(IEnumerable<string> roles, string requiredPermission, CancellationToken ct = default);

    /// <summary>
    /// Clears permission cache for a specific role (e.g. after update).
    /// </summary>
    Task InvalidateRoleCacheAsync(string roleName);
}