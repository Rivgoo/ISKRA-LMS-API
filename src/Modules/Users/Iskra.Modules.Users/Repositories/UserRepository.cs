using Iskra.Core.Domain.Entities;
using Iskra.Infrastructure.Shared.Persistence;
using Iskra.Infrastructure.Shared.Repositories;
using Iskra.Modules.Users.Abstractions.Models.Responses;
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

    public async Task<UserResponse?> GetFullResponseAsync(Guid userId, CancellationToken ct)
    {
        return await Entities.AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new UserResponse(
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                u.MiddleName,
                u.IsActive,
                u.IsEmailConfirmed,
                u.UserRoles.Select(ur => ur.Role!.Name).ToList(),
                u.CreatedAt,
                u.UpdatedAt
            ))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<UserPublicResponse?> GetPublicResponseAsync(Guid userId, CancellationToken ct)
    {
        return await Entities.AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new UserPublicResponse(
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                u.MiddleName
            ))
            .FirstOrDefaultAsync(ct);
    }
}