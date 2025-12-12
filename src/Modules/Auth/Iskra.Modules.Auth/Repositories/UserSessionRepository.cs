using Iskra.Core.Domain.Entities;
using Iskra.Infrastructure.Persistence.Repositories;
using Iskra.Infrastructure.Shared.Persistence;
using Iskra.Modules.Auth.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Iskra.Modules.Auth.Repositories;

internal sealed class UserSessionRepository(AppDbContextBase dbContext)
    : EntityRepository<UserSession, Guid>(dbContext), IUserSessionRepository
{
    public async Task<UserSession?> GetByTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await Entities.FirstOrDefaultAsync(s => s.RefreshToken == refreshToken, cancellationToken);
    }

    public async Task RevokeAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        await Entities
            .Where(s => s.Id == sessionId)
            .ExecuteUpdateAsync(updates => updates
                .SetProperty(s => s.IsRevoked, true)
                .SetProperty(s => s.RevokedAt, DateTimeOffset.UtcNow),
                cancellationToken);
    }

    public async Task RevokeAllExceptAsync(Guid userId, Guid currentSessionId, CancellationToken cancellationToken = default)
    {
        await Entities
            .Where(s => s.UserId == userId && s.Id != currentSessionId && !s.IsRevoked)
            .ExecuteUpdateAsync(updates => updates
                .SetProperty(s => s.IsRevoked, true)
                .SetProperty(s => s.RevokedAt, DateTimeOffset.UtcNow),
                cancellationToken);
    }

    public async Task<int> DeleteExpiredSessionsAsync(DateTimeOffset cutoffDate, CancellationToken ct = default)
    {
        return await Entities
            .Where(s => (s.IsRevoked && s.RevokedAt < cutoffDate) || (s.ExpiresAt < cutoffDate))
            .ExecuteDeleteAsync(ct);
    }
}