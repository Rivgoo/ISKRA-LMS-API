using Iskra.Core.Domain.Common;

namespace Iskra.Core.Domain.Entities;

/// <summary>
/// Represents a security role (e.g., Student, Teacher, Administrator).
/// </summary>
public class Role : AuditableEntity<Guid>
{
    /// <summary>
    /// Gets or sets the unique name of the role (e.g., "Admin").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a description of what this role allows.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is a system role that cannot be deleted.
    /// </summary>
    public bool IsSystem { get; set; }
}