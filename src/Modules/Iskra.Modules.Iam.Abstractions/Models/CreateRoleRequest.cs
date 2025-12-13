namespace Iskra.Modules.Iam.Abstractions.Models;

public record CreateRoleRequest(string Name, string? Description, List<string> Permissions);