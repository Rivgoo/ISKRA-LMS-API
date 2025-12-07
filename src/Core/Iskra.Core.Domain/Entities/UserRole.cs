using Iskra.Core.Domain.Common;

namespace Iskra.Core.Domain.Entities;

/// <summary>
/// Represents the association between a User and a Role.
/// </summary>
public class UserRole : IEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid RoleId { get; set; }
    public Role? Role { get; set; }
}