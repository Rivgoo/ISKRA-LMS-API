using Iskra.Core.Domain.Entities;
using Iskra.Infrastructure.Persistence.Repositories;
using Iskra.Infrastructure.Shared.Persistence;
using Iskra.Modules.Users.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Iskra.Modules.Users.Repositories;

internal sealed class RoleRepository(AppDbContextBase dbContext)
    : EntityRepository<Role, Guid>(dbContext), IRoleRepository
{
    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Entities.AsNoTracking().FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }
}