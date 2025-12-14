using Iskra.Core.Domain.Entities;
using Iskra.Infrastructure.Shared.Persistence;
using Iskra.Infrastructure.Shared.Repositories;
using Iskra.Modules.Iam.Abstractions.Models;
using Iskra.Modules.Iam.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Iskra.Modules.Iam.Repositories;

internal sealed class RoleRepository(AppDbContextBase dbContext)
    : EntityRepository<Role, Guid>(dbContext), IRoleRepository
{
    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Entities.AsNoTracking().FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }

    public async Task<List<RoleDto>> GetAllWithCountsAsync(CancellationToken ct = default)
    {
        return await Entities.AsNoTracking()
            .Select(r => new RoleDto(
                r.Id,
                r.Name,
                r.Description,
                r.IsSystem,
                DbContext.Set<UserRole>().Count(ur => ur.RoleId == r.Id)
            ))
            .ToListAsync(ct);
    }
}