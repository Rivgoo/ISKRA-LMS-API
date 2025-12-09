using Iskra.Application.Abstractions.Repositories;

namespace Iskra.Infrastructure.Shared.Persistence;

internal sealed class UnitOfWork(AppDbContextBase dbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => await dbContext.SaveChangesAsync(cancellationToken);
}