using Iskra.Api.Abstractions.Services;
using Iskra.Modules.Iam.Abstractions.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Iskra.Modules.Auth.Services;

internal sealed class CurrentUser(
    IHttpContextAccessor httpContextAccessor,
    IPermissionService permissionService)
    : ICurrentUser
{
    private readonly ClaimsPrincipal? _user = httpContextAccessor.HttpContext?.User;

    public Guid? Id => Guid.TryParse(_user?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var guid) ? guid : null;

    public string? Email => _user?.FindFirst(ClaimTypes.Email)?.Value;

    public bool IsAuthenticated => _user?.Identity?.IsAuthenticated is true;

    public IReadOnlySet<string> Roles => _roles ??= GetRoles();

    private HashSet<string>? _roles;

    public bool IsInRole(string role)
    {
        return Roles.Contains(role);
    }

    public async Task<bool> HasPermissionAsync(string permission)
    {
        if (!IsAuthenticated) return false;

        return await permissionService.HasPermissionAsync(Roles, permission);
    }

    public async Task<bool> CanAccessAsync(Guid resourceOwnerId, string requiredPermission)
    {
        var currentId = Id;
        if (currentId == null) return false;

        // 1. Self Access
        if (currentId == resourceOwnerId) return true;

        // 2. Permission Check
        return await HasPermissionAsync(requiredPermission);
    }

    private HashSet<string> GetRoles()
    {
        if (_user == null) return [];

        return _user.FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}