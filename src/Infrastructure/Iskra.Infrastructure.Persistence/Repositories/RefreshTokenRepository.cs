using Iskra.Application.Abstractions.Repositories;
using Iskra.Core.Domain.Entities;
using Iskra.Infrastructure.Persistence.Repositories.Base;
using Iskra.Infrastructure.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iskra.Infrastructure.Persistence.Repositories;

internal sealed class RefreshTokenRepository(AppDbContextBase dbContext)
    : EntityRepository<RefreshToken, Guid>(dbContext), IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await Entities.FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }
}