using Iskra.Modules.Iam.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Iskra.Modules.Auth.Authorization;

internal sealed class PermissionAuthorizationHandler(IPermissionService permissionService)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var roles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value);

        if (await permissionService.HasPermissionAsync(roles, requirement.Permission))
            context.Succeed(requirement);
    }
}