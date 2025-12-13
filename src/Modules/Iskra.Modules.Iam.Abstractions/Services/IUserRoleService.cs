using Iskra.Application.Results;
using Iskra.Core.Domain.Entities;

namespace Iskra.Modules.Iam.Abstractions.Services;

/// <summary>
/// Defines business logic for managing user role assignments.
/// </summary>
public interface IUserRoleService
{
    /// <summary>
    /// Assigns a specific role to a user.
    /// </summary>
    Task<Result> AssignRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a specific role from a user.
    /// </summary>
    Task<Result> RevokeRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all roles currently assigned to a user.
    /// </summary>
    Task<Result<ICollection<Role>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);
}