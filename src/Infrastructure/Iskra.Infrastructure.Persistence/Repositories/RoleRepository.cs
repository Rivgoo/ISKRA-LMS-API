using Iskra.Application.Abstractions.Repositories;
using Iskra.Core.Domain.Entities;
using Iskra.Infrastructure.Persistence.Repositories.Base;
using Iskra.Infrastructure.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iskra.Infrastructure.Persistence.Repositories;

internal sealed class RoleRepository(AppDbContextBase dbContext)
    : EntityRepository<Role, Guid>(dbContext), IRoleRepository
{
    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Entities.AsNoTracking().FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }
}