using Iskra.Core.Domain.Common;

namespace Iskra.Core.Domain.Entities;

/// <summary>
/// Represents a security role (e.g., Student, Teacher, Administrator).
/// </summary>
public class Role : AuditableEntity<Guid>
{
    // Required by EF Core
    private Role() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Role"/> class.
    /// </summary>
    /// <param name="name">The unique name of the role.</param>
    /// <param name="description">The optional description.</param>
    /// <param name="isSystem">If true, prevents deletion or critical modification.</param>
    public Role(string name, string? description, bool isSystem)
    {
        Guard.NotNullOrWhiteSpace(name);

        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        IsSystem = isSystem;
    }

    /// <summary>
    /// Gets the unique name of the role (e.g., "Admin").
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a description of what this role allows.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this is a system role that cannot be deleted.
    /// </summary>
    public bool IsSystem { get; private set; }

    /// <summary>
    /// Updates the details of the role.
    /// </summary>
    /// <param name="name">The new name.</param>
    /// <param name="description">The new description.</param>
    /// <exception cref="InvalidOperationException">Thrown if attempting to rename a system role.</exception>
    public void UpdateDetails(string name, string? description)
    {
        Guard.NotNullOrWhiteSpace(name);

        if (IsSystem && !Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Cannot rename a system role.");

        Name = name;
        Description = description;
    }
}