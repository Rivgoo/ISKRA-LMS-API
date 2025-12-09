using Iskra.Core.Domain.Entities;
using Iskra.Infrastructure.Persistence.Repositories;
using Iskra.Infrastructure.Shared.Persistence;
using Iskra.Modules.Users.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Iskra.Modules.Users.Repositories;

/// <summary>
/// Implements the repository for managing the User-Role relationship.
/// </summary>
internal sealed class UserRoleRepository(AppDbContextBase dbContext)
    : Repository<UserRole>(dbContext), IUserRoleRepository
{
    public void Add(UserRole userRole)
    {
        Entities.Add(userRole);
    }

    public void Remove(UserRole userRole)
    {
        Entities.Remove(userRole);
    }

    public async Task<bool> UserHasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        return await Entities.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken);
    }

    public async Task<ICollection<Role>> GetRolesForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Entities
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role!)
            .ToListAsync(cancellationToken);
    }
}