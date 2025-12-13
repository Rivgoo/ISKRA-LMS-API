using Iskra.Application.Results;
using Iskra.Core.Domain.Entities;
using Iskra.Modules.Iam.Abstractions.Models;

namespace Iskra.Modules.Iam.Abstractions.Services;

public interface IRoleManager
{
    Task<Result<Role>> CreateRoleAsync(CreateRoleRequest request, CancellationToken ct = default);
    Task<Result<Role>> UpdateRoleAsync(Guid roleId, UpdateRoleRequest request, CancellationToken ct = default);
    Task<Result> DeleteRoleAsync(Guid roleId, CancellationToken ct = default);

    Task<Result> UpdatePermissionsAsync(Guid roleId, List<string> newPermissions, CancellationToken ct = default);

    Task<Result<RoleDto>> GetRoleByIdAsync(Guid roleId, CancellationToken ct = default);
    Task<Result<List<RoleDto>>> GetAllRolesAsync(CancellationToken ct = default);
    Task<Result<List<string>>> GetPermissionsAsync(Guid roleId, CancellationToken ct = default);
}