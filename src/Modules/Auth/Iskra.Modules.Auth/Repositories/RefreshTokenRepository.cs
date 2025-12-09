using Iskra.Core.Domain.Entities;
using Iskra.Infrastructure.Persistence.Repositories;
using Iskra.Infrastructure.Shared.Persistence;
using Iskra.Modules.Auth.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Iskra.Modules.Auth.Repositories;

internal sealed class RefreshTokenRepository(AppDbContextBase dbContext)
    : EntityRepository<RefreshToken, Guid>(dbContext), IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await Entities.FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }
}