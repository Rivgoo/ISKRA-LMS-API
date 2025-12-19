using Iskra.Application.Abstractions.Repositories;
using Iskra.Core.Domain.Entities;
using Iskra.Modules.Users.Abstractions.Models.Responses;

namespace Iskra.Modules.Users.Abstractions.Repositories;

/// <summary>
/// Defines the contract for a repository that manages User entities.
/// </summary>
public interface IUserRepository : IEntityOperations<User, Guid>
{
    /// <summary>
    /// Asynchronously retrieves a user by their unique email address.
    /// </summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously updates the active status of a user directly in the database.
    /// </summary>
    /// <param name="userId">The ID of the user to update.</param>
    /// <param name="isActive">The new active status.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The number of rows affected.</returns>
    Task<int> SetActiveStatusAsync(Guid userId, bool isActive, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously updates the password hash for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose password hash will be updated.</param>
    /// <param name="newPasswordHash">The new password hash to assign to the user. Cannot be null or empty.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of records affected by
    /// the update.</returns>
    Task<int> UpdatePasswordHashAsync(Guid userId, string newPasswordHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously retrieves the complete response data for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose response data is to be retrieved.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="UserResponse"/> object
    /// with the user's full response data, or <see langword="null"/> if no response is found for the specified user.</returns>
    Task<UserResponse?> GetFullResponseAsync(Guid userId, CancellationToken ct);

    /// <summary>
    /// Asynchronously retrieves the public profile information for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose public information is to be retrieved.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="UserPublicResponse"/>
    /// with the user's public information if found; otherwise, <see langword="null"/>.</returns>
    Task<UserPublicResponse?> GetPublicResponseAsync(Guid userId, CancellationToken ct);
}