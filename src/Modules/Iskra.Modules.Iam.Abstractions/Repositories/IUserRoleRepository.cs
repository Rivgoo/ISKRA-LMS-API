using Iskra.Application.Abstractions.Repositories;
using Iskra.Core.Domain.Entities;

namespace Iskra.Modules.Iam.Abstractions.Repositories;

/// <summary>
/// Defines a contract for a repository that manages the UserRole join entity.
/// </summary>
public interface IUserRoleRepository : IRepository
{
    /// <summary>
    /// Adds a role to a user.
    /// </summary>
    void Add(UserRole userRole);

    /// <summary>
    /// Removes a role from a user.
    /// </summary>
    void Remove(UserRole userRole);

    /// <summary>
    /// Asynchronously checks if a user has a specific role.
    /// </summary>
    Task<bool> UserHasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously gets all roles assigned to a user.
    /// </summary>
    Task<ICollection<Role>> GetRolesForUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously counts the number of users assigned to the specified role.
    /// </summary>
    /// <param name="roleId">The unique identifier of the role for which to count users.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of users assigned to the
    /// specified role.</returns>
    Task<int> CountUsersInRoleAsync(Guid roleId, CancellationToken ct = default);
}