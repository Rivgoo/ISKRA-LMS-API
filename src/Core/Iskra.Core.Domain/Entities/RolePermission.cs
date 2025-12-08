using Iskra.Core.Domain.Common;

namespace Iskra.Core.Domain.Entities;

/// <summary>
/// Represents a specific permission granted to a role.
/// </summary>
public class RolePermission : IEntity
{
    public Guid RoleId { get; set; }
    public Role? Role { get; set; }

    /// <summary>
    /// The permission string (e.g., "users.read").
    /// Validated against Iskra.Core.Permissions.
    /// </summary>
    public string Permission { get; set; } = string.Empty;
}