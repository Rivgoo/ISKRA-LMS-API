using Iskra.Application.Abstractions.Repositories;
using Iskra.Application.Errors;
using Iskra.Application.Results;
using Iskra.Core.Domain.Entities;
using Iskra.Modules.Iam.Abstractions;
using Iskra.Modules.Iam.Abstractions.Errors;
using Iskra.Modules.Iam.Abstractions.Repositories;
using Iskra.Modules.Iam.Abstractions.Services;

namespace Iskra.Modules.Iam.Services;

internal sealed class RolePermissionService(
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository,
    IUnitOfWork unitOfWork)
    : IRolePermissionService
{
    private static readonly HashSet<string> _validSystemPermissions
        = [.. PermissionDiscovery.GetAllPermissions()];

    public Result<IEnumerable<string>> GetAllSystemPermissions()
    {
        return Result<IEnumerable<string>>.Ok(_validSystemPermissions);
    }

    public async Task<Result<HashSet<string>>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        if (!await roleRepository.ExistsByIdAsync(roleId, cancellationToken))
            return EntityErrors<Role, Guid>.NotFoundById(roleId);

        var permissions = await permissionRepository.GetPermissionsForRoleAsync(roleId, cancellationToken);
        return Result<HashSet<string>>.Ok(permissions);
    }

    public async Task<Result> UpdateRolePermissionsAsync(Guid roleId, IEnumerable<string> permissions, CancellationToken cancellationToken = default)
    {
        // 1. Check Role Exists
        var role = await roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (role is null)
            return EntityErrors<Role, Guid>.NotFoundById(roleId);

        // 2. Prevent modifying System Roles (Optional safety check)
        if (role.IsSystem)
            return RoleErrors.SystemRoleModification;

        // 3. Validate Permissions Exist in Code
        var invalidPermissions = permissions.Where(p => !_validSystemPermissions.Contains(p)).ToList();
        if (invalidPermissions.Count > 0)
            return PermissionsErrors.InvalidPermissions(invalidPermissions);

        // 4. Update Database
        await permissionRepository.ReplacePermissionsAsync(roleId, permissions, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}