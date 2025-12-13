namespace Iskra.Modules.Iam.Abstractions.Models;

public record RoleDto(Guid Id, string Name, string? Description, bool IsSystem, int UserCount);