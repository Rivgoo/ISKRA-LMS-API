using Iskra.Application.Errors;

namespace Iskra.Modules.Iam.Abstractions.Errors;

public static class UserRoleErrors
{
    public static Error AlreadyAssigned(string roleName)
        => Error.Conflict("UserRole.AlreadyAssigned", "User already has the role '{0}'.", roleName);

    public static Error NotAssigned(string roleName)
        => Error.BadRequest("UserRole.NotAssigned", "User does not have the role '{0}'.", roleName);
}