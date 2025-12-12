using Iskra.Core.Contracts.Constants;
using Iskra.Core.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace Iskra.Modules.Auth.Authorization;

internal sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // 1. Check if user is authenticated at all
        if (context.User.Identity is not { IsAuthenticated: true })
            return Task.CompletedTask;

        // 2. Super Admin Check
        if (context.User.HasClaim(c =>
            c.Type == AuthConstants.PermissionsClaimType &&
            c.Value == IskraPermissions.SystemFullAccess))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // 3. Specific Permission Check
        if (context.User.HasClaim(c =>
            c.Type == AuthConstants.PermissionsClaimType &&
            c.Value == requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}