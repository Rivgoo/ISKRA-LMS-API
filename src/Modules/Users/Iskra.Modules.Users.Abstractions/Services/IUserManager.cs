using Iskra.Application.Results;
using Iskra.Modules.Users.Abstractions.Models;

namespace Iskra.Modules.Users.Abstractions.Services;

/// <summary>
/// Provides a high-level facade for managing user lifecycles, including registration, 
/// updates, password management, and role assignments.
/// Ensures transactional integrity and business rule enforcement.
/// </summary>
public interface IUserManager
{
    /// <summary>
    /// Registers a new user, assigns an initial role, and handles password hashing.
    /// If role assignment fails, the user creation is rolled back.
    /// </summary>
    Task<Result<Guid>> RegisterUserAsync(RegisterUserRequest request, CancellationToken ct = default);

    /// <summary>
    /// Updates the user's profile information (names).
    /// </summary>
    Task<Result> UpdateProfileAsync(Guid userId, UpdateUserRequest request, CancellationToken ct = default);

    /// <summary>
    /// Changes the user's password, verifying the old password first.
    /// </summary>
    Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct = default);

    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    Task<Result> AssignRoleAsync(Guid userId, string roleName, CancellationToken ct = default);

    /// <summary>
    /// Revokes a role from a user.
    /// </summary>
    Task<Result> RevokeRoleAsync(Guid userId, string roleName, CancellationToken ct = default);

    /// <summary>
    /// Completely removes a user and all associated data (roles, sessions, etc.) from the system.
    /// </summary>
    Task<Result> DeleteUserAsync(Guid userId, CancellationToken ct = default);
}