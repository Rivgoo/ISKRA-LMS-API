using Iskra.Application.Errors;

namespace Iskra.Modules.Iam.Abstractions.Errors;

public static class PermissionsErrors
{
    public static Error InvalidPermissions(List<string> permissions)
        => Error.BadRequest(
            "Permissions.Invalid",
            "The following permissions are unknown to the system: {0}",
            string.Join(", ", permissions));
}