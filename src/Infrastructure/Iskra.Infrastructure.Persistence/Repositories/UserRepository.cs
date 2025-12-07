using Iskra.Application.Abstractions.Repositories;
using Iskra.Core.Domain.Entities;
using Iskra.Infrastructure.Persistence.Repositories.Base;
using Iskra.Infrastructure.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Iskra.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(AppDbContextBase dbContext)
    : EntityRepository<User, Guid>(dbContext), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Entities.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<int> SetActiveStatusAsync(Guid userId, bool isActive)
    {
        return await Entities
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(updates => updates.SetProperty(u => u.IsActive, isActive));
    }

    public async Task<int> UpdatePasswordHashAsync(Guid userId, string newPasswordHash)
    {
        return await Entities
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(updates => updates.SetProperty(u => u.PasswordHash, newPasswordHash));
    }
}