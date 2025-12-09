namespace Iskra.Application.Errors.Core;

public static class PermissionsErrors
{
    public static Error InvalidPermissions(List<string> permissions)
        => Error.BadRequest(
            "Permissions.Invalid",
            "The following permissions are unknown to the system: {0}",
            string.Join(", ", permissions));
}