using Iskra.Application.Abstractions.Repositories.Base;
using Iskra.Infrastructure.Shared.Persistence;

namespace Iskra.Infrastructure.Persistence;

internal sealed class UnitOfWork(AppDbContextBase dbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await dbContext.SaveChangesAsync(cancellationToken);
}