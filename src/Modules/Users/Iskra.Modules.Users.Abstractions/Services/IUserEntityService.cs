using Iskra.Application.Abstractions.Services;
using Iskra.Application.Results;
using Iskra.Core.Domain.Entities;

namespace Iskra.Modules.Users.Abstractions.Services;

/// <summary>
/// Defines service operations specific to the User entity.
/// </summary>
public interface IUserEntityService : IEntityService<User, Guid>
{
    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the user's password hash.
    /// </summary>
    Task<Result> UpdatePasswordHashAsync(Guid userId, string newPasswordHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Activates or deactivates a user account.
    /// </summary>
    Task<Result> SetActiveStatusAsync(Guid userId, bool isActive, CancellationToken cancellationToken = default);
}