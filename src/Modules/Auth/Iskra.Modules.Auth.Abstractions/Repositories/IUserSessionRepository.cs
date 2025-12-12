using Iskra.Application.Abstractions.Repositories;
using Iskra.Core.Domain.Entities;

namespace Iskra.Modules.Auth.Abstractions.Repositories;

public interface IUserSessionRepository : IEntityOperations<UserSession, Guid>
{
    /// <summary>
    /// Retrieves a session by the refresh token string.
    /// </summary>
    Task<UserSession?> GetByTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes (terminates) a specific session.
    /// Used for "Log out" functionality.
    /// </summary>
    Task RevokeAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes all other sessions for the user except the current one.
    /// Used for "Log out from all other devices".
    /// </summary>
    Task RevokeAllExceptAsync(Guid userId, Guid currentSessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hard deletes sessions that expired or were revoked before the specified cutoff date.
    /// </summary>
    /// <returns>Number of deleted records.</returns>
    Task<int> DeleteExpiredSessionsAsync(DateTimeOffset cutoffDate, CancellationToken ct = default);
}