using Iskra.Core.Domain.Common;

namespace Iskra.Core.Domain.Entities;

/// <summary>
/// Represents a specific permission granted to a role.
/// </summary>
public class RolePermission : IEntity
{
    // Required by EF Core
    private RolePermission() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RolePermission"/> class.
    /// </summary>
    public RolePermission(Guid roleId, string permission)
    {
        Guard.NotEmpty(roleId);
        Guard.NotNullOrWhiteSpace(permission);

        RoleId = roleId;
        Permission = permission;
    }

    public Guid RoleId { get; private set; }
    public Role? Role { get; private set; }

    /// <summary>
    /// The permission string (e.g., "users.read").
    /// </summary>
    public string Permission { get; private set; } = string.Empty;
}