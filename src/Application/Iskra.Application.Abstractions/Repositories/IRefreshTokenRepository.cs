using Iskra.Application.Abstractions.Repositories.Base;
using Iskra.Core.Domain.Entities;

namespace Iskra.Application.Abstractions.Repositories;

/// <summary>
/// Defines the contract for a repository that manages RefreshToken entities.
/// </summary>
public interface IRefreshTokenRepository : IEntityOperations<RefreshToken, Guid>
{
    /// <summary>
    /// Asynchronously retrieves a refresh token by its token string.
    /// </summary>
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
}