using Iskra.Application.Abstractions.Caching;
using Iskra.Modules.Iam.Abstractions;
using Iskra.Modules.Iam.Abstractions.Repositories;
using Iskra.Modules.Iam.Abstractions.Services;

namespace Iskra.Modules.Iam.Services;

internal sealed class PermissionService(
    ICacheService cache,
    IRoleRepository roleRepository,
    IPermissionRepository permissionRepository
    ) : IPermissionService
{
    public async Task<bool> HasPermissionAsync(IEnumerable<string> roleNames, string requiredPermission, 
        CancellationToken ct = default)
    {
        var allPermissions = new HashSet<string>();

        foreach (var role in roleNames)
        {
            var permissions = await cache.GetOrCreateAsync(
               $"iam:permissions:{role}",
               async token =>
               {
                   var r = await roleRepository.GetByNameAsync(role, token);
                   if (r == null) return [];
                   return await permissionRepository.GetPermissionsForRoleAsync(r.Id, token);
               },

               CacheEntryOptions.Absolute(TimeSpan.FromMinutes(30)),
               ct);

            if (permissions != null) allPermissions.UnionWith(permissions);
        }

        return allPermissions.Contains(IskraPermissions.SystemFullAccess) ||
               allPermissions.Contains(requiredPermission);
    }

    public Task InvalidateRoleCacheAsync(string roleName)
    {
        cache.Remove($"iam:permissions:{roleName}");
        return Task.CompletedTask;
    }
}