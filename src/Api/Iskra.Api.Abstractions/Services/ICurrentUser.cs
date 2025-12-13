namespace Iskra.Api.Abstractions.Services;

public interface ICurrentUser
{
    Guid? Id { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets all roles assigned to the user from token claims.
    /// </summary>
    IReadOnlySet<string> Roles { get; }

    /// <summary>
    /// Checks if the user is in a specific role (case-insensitive).
    /// </summary>
    bool IsInRole(string role);

    /// <summary>
    /// Asynchronously checks if the current user has a specific permission.
    /// Uses cached permission lookup service.
    /// </summary>
    Task<bool> HasPermissionAsync(string permission);

    /// <summary>
    /// Helper to check if the user is allowed to access a specific resource owner.
    /// Returns true if (CurrentUser.Id == resourceOwnerId) OR (CurrentUser has requiredPermission).
    /// </summary>
    Task<bool> CanAccessAsync(Guid resourceOwnerId, string requiredPermission);
}