using Iskra.Application.Errors;

namespace Iskra.Modules.Iam.Abstractions.Errors;

public static class RoleErrors
{
    public static Error NotFound(Guid roleId)
        => Error.NotFound("Role.NotFound", "Role with ID '{0}' not found.", roleId);

    public static Error NotFound(string roleName)
        => Error.NotFound("Role.NotFound", "Role with name '{0}' not found.", roleName);

    public static Error AlreadyExists(string name)
        => Error.Conflict("Role.AlreadyExists", "Role with name '{0}' already exists.", name);

    public static Error SystemRoleModification
        => Error.Conflict("Role.SystemProtected", "Cannot modify or delete a system role.");

    public static Error CannotDeleteUsedRole(int userCount)
        => Error.Conflict("Role.InUse", "Cannot delete role because it is assigned to {0} users.", userCount);

    public static Error UnknownPermissions(IEnumerable<string> permissions)
        => Error.BadRequest("Role.UnknownPermissions", "The following permissions are unknown: {0}", string.Join(", ", permissions));
}