using Iskra.Application.Abstractions.Repositories;
using Iskra.Application.Errors;
using Iskra.Application.Results;
using Iskra.Core.Domain.Entities;
using Iskra.Modules.Iam.Abstractions.Errors;
using Iskra.Modules.Iam.Abstractions.Repositories;
using Iskra.Modules.Iam.Abstractions.Services;
using Iskra.Modules.Users.Abstractions.Repositories;

namespace Iskra.Modules.Iam.Services;

internal sealed class UserRoleService(
	IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUserRoleRepository userRoleRepository,
    IUnitOfWork unitOfWork)
    : IUserRoleService
{
    public async Task<Result> AssignRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        // 1. Validate User Exists
        if (!await userRepository.ExistsByIdAsync(userId, cancellationToken))
            return EntityErrors<User, Guid>.NotFoundById(userId);

        // 2. Validate Role Exists
        var role = await roleRepository.GetByNameAsync(roleName, cancellationToken);
        if (role is null)
            return RoleErrors.NotFound(roleName);

        // 3. Check Idempotency (Already exists?)
        if (await userRoleRepository.UserHasRoleAsync(userId, role.Id, cancellationToken))
            return UserRoleErrors.AlreadyAssigned(roleName);

        // 4. Assign
        var userRole = new UserRole(userId, role.Id);
        userRoleRepository.Add(userRole);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }

    public async Task<Result> RevokeRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        // 1. Validate User Exists
        if (!await userRepository.ExistsByIdAsync(userId, cancellationToken))
            return EntityErrors<User, Guid>.NotFoundById(userId);

        // 2. Validate Role Exists
        var role = await roleRepository.GetByNameAsync(roleName, cancellationToken);
        if (role is null)
            return RoleErrors.NotFound(roleName);

        // 3. Check Existence
        if (!await userRoleRepository.UserHasRoleAsync(userId, role.Id, cancellationToken))
            return UserRoleErrors.NotAssigned(roleName);

        // 4. Revoke
        var userRole = new UserRole(userId, role.Id);
        userRoleRepository.Remove(userRole);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }

    public async Task<Result<ICollection<Role>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (!await userRepository.ExistsByIdAsync(userId, cancellationToken))
            return EntityErrors<User, Guid>.NotFoundById(userId);

        var roles = await userRoleRepository.GetRolesForUserAsync(userId, cancellationToken);

        return Result<ICollection<Role>>.Ok(roles);
    }
}