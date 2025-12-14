using Iskra.Core.Domain.Common;

namespace Iskra.Core.Domain.Entities;

/// <summary>
/// Represents the association between a User and a Role.
/// </summary>
public class UserRole : IEntity
{
    // Required by EF Core
    private UserRole() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRole"/> class.
    /// </summary>
    public UserRole(Guid userId, Guid roleId)
    {
        Guard.NotEmpty(userId);
        Guard.NotEmpty(roleId);

        UserId = userId;
        RoleId = roleId;
    }

    public Guid UserId { get; private set; }
    public User? User { get; private set; }

    public Guid RoleId { get; private set; }
    public Role? Role { get; private set; }
}