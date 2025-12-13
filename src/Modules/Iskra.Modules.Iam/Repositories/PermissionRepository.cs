using Iskra.Core.Domain.Entities;
using Iskra.Infrastructure.Persistence.Repositories;
using Iskra.Infrastructure.Shared.Persistence;
using Iskra.Modules.Iam.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Iskra.Modules.Iam.Repositories;

internal sealed class PermissionRepository(AppDbContextBase dbContext)
    : Repository<RolePermission>(dbContext), IPermissionRepository
{
    public async Task<HashSet<string>> GetPermissionsForRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var permissions = await Entities
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.Permission)
            .ToListAsync(cancellationToken);

        return [.. permissions];
    }

    public async Task ReplacePermissionsAsync(Guid roleId, IEnumerable<string> newPermissions, CancellationToken cancellationToken = default)
    {
        var existing = Entities.Where(rp => rp.RoleId == roleId);
        Entities.RemoveRange(existing);

        var entitiesToAdd = newPermissions
            .Distinct()
            .Select(p => new RolePermission { RoleId = roleId, Permission = p });

        Entities.AddRange(entitiesToAdd);

        await Task.CompletedTask;
    }
}