using Microsoft.AspNetCore.Authorization;

namespace Iskra.Modules.Auth.Authorization;

internal sealed class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}