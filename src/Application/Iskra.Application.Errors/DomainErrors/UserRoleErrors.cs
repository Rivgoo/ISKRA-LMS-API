namespace Iskra.Application.Errors.DomainErrors;

public static class UserRoleErrors
{
    public static Error RoleNotFound(string roleName)
        => Error.NotFound("Role.NotFound", "The role '{0}' does not exist.", roleName);

    public static Error RoleSystemProtected()
        => Error.Conflict("Role.SystemProtected", "Cannot modify permissions of a system role.");

    public static Error AlreadyAssigned(string roleName)
        => Error.Conflict("UserRole.AlreadyAssigned", "User already has the role '{0}'.", roleName);

    public static Error NotAssigned(string roleName)
        => Error.BadRequest("UserRole.NotAssigned", "User does not have the role '{0}'.", roleName);
}