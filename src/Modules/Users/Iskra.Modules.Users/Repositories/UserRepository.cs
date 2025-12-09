using Iskra.Core.Domain.Entities;
using Iskra.Infrastructure.Persistence.Repositories;
using Iskra.Infrastructure.Shared.Persistence;
using Iskra.Modules.Users.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Iskra.Modules.Users.Repositories;

internal sealed class UserRepository(AppDbContextBase dbContext)
    : EntityRepository<User, Guid>(dbContext), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Entities.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<int> SetActiveStatusAsync(Guid userId, bool isActive, CancellationToken cancellationToken = default)
    {
        return await Entities
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(updates => updates.SetProperty(u => u.IsActive, isActive), cancellationToken);
    }

    public async Task<int> UpdatePasswordHashAsync(Guid userId, string newPasswordHash, CancellationToken cancellationToken = default)
    {
        return await Entities
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(updates => updates.SetProperty(u => u.PasswordHash, newPasswordHash), cancellationToken);
    }
}