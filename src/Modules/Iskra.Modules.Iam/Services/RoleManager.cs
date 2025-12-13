using Iskra.Application.Abstractions.Repositories;
using Iskra.Application.Results;
using Iskra.Core.Domain.Entities;
using Iskra.Modules.Iam.Abstractions;
using Iskra.Modules.Iam.Abstractions.Errors;
using Iskra.Modules.Iam.Abstractions.Models;
using Iskra.Modules.Iam.Abstractions.Repositories;
using Iskra.Modules.Iam.Abstractions.Services;

namespace Iskra.Modules.Iam.Services;

internal sealed class RoleManager(
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository,
    IUserRoleRepository userRoleRepository,
    IPermissionService permissionService,
    IUnitOfWork unitOfWork)
    : IRoleManager
{
    public async Task<Result<Role>> CreateRoleAsync(CreateRoleRequest request, CancellationToken ct = default)
    {
        var existing = await roleRepository.GetByNameAsync(request.Name, ct);
        if (existing != null)
            return Result<Role>.Bad(RoleErrors.AlreadyExists(request.Name));

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            IsSystem = false
        };

        roleRepository.Add(role);

        var validPermissions = PermissionDiscovery.GetAllPermissions();
        var invalid = request.Permissions.Except(validPermissions).ToList();

        if (invalid.Count > 0)
            return Result<Role>.Bad(RoleErrors.UnknownPermissions(invalid));

        if (request.Permissions.Count > 0)
            await permissionRepository.ReplacePermissionsAsync(role.Id, request.Permissions, ct);

        await unitOfWork.SaveChangesAsync(ct);
        return Result<Role>.Ok(role);
    }

    public async Task<Result<Role>> UpdateRoleAsync(Guid roleId, UpdateRoleRequest request, CancellationToken ct = default)
    {
        var role = await roleRepository.GetByIdAsync(roleId, ct);
        if (role == null) return RoleErrors.NotFound(roleId);

        if (role.IsSystem)
            return RoleErrors.SystemRoleModification;

        // 1. Rename Logic
        string oldName = role.Name;
        bool nameChanged = !string.Equals(oldName, request.Name, StringComparison.OrdinalIgnoreCase);

        if (nameChanged)
        {
            // Check uniqueness
            var existingName = await roleRepository.GetByNameAsync(request.Name, ct);

            if (existingName != null && existingName.Id != roleId)
                return RoleErrors.AlreadyExists(request.Name);

            role.Name = request.Name;
        }

        role.Description = request.Description;

        roleRepository.Update(role);
        await unitOfWork.SaveChangesAsync(ct);

        // 2. Cache Invalidation
        if (nameChanged)
            await permissionService.InvalidateRoleCacheAsync(oldName);

        return Result<Role>.Ok(role);
    }

    public async Task<Result> UpdatePermissionsAsync(Guid roleId, List<string> newPermissions, CancellationToken ct = default)
    {
        var role = await roleRepository.GetByIdAsync(roleId, ct);
        if (role == null) return RoleErrors.NotFound(roleId);

        if (role.IsSystem)
            return RoleErrors.SystemRoleModification;

        var validPermissions = PermissionDiscovery.GetAllPermissions();
        var invalid = newPermissions.Except(validPermissions).ToList();

        if (invalid.Count > 0)
            return RoleErrors.UnknownPermissions(invalid);

        await permissionRepository.ReplacePermissionsAsync(roleId, newPermissions, ct);
        await unitOfWork.SaveChangesAsync(ct);

        await permissionService.InvalidateRoleCacheAsync(role.Name);

        return Result.Ok();
    }

    public async Task<Result> DeleteRoleAsync(Guid roleId, CancellationToken ct = default)
    {
        var role = await roleRepository.GetByIdAsync(roleId, ct);
        if (role == null) return RoleErrors.NotFound(roleId);

        if (role.IsSystem)
            return RoleErrors.SystemRoleModification;

        // 1. Check Usage
        var userCount = await userRoleRepository.CountUsersInRoleAsync(roleId, ct);
        if (userCount > 0)
            return RoleErrors.CannotDeleteUsedRole(userCount);

        // 2. Delete
        roleRepository.Remove(role);
        await unitOfWork.SaveChangesAsync(ct);

        await permissionService.InvalidateRoleCacheAsync(role.Name);

        return Result.Ok();
    }

    public async Task<Result<RoleDto>> GetRoleByIdAsync(Guid roleId, CancellationToken ct = default)
    {
        var role = await roleRepository.GetByIdAsync(roleId, ct);
        if (role == null) return RoleErrors.NotFound(roleId);

        var userCount = await userRoleRepository.CountUsersInRoleAsync(roleId, ct);
        return new RoleDto(role.Id, role.Name, role.Description, role.IsSystem, userCount);
    }

    public async Task<Result<List<RoleDto>>> GetAllRolesAsync(CancellationToken ct = default)
    {
        var dtos = await roleRepository.GetAllWithCountsAsync(ct);
        return Result<List<RoleDto>>.Ok(dtos);
    }

    public async Task<Result<List<string>>> GetPermissionsAsync(Guid roleId, CancellationToken ct = default)
    {
        if (!await roleRepository.ExistsByIdAsync(roleId, ct))
            return RoleErrors.NotFound(roleId);

        var perms = await permissionRepository.GetPermissionsForRoleAsync(roleId, ct);
        return perms.ToList();
    }
}