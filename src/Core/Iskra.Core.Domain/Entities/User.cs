using Iskra.Core.Domain.Common;

namespace Iskra.Core.Domain.Entities;

/// <summary>
/// Represents a registered user within the system.
/// </summary>
public class User : AuditableEntity<Guid>
{
    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's middle name (optional).
    /// </summary>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Gets or sets the unique email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the BCrypt/Argon2 hash of the password.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the user is allowed to log in.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating if the email address has been verified.
    /// </summary>
    public bool IsEmailConfirmed { get; set; } = false;

    // --- Navigation Properties ---

    /// <summary>
    /// Roles assigned to this user.
    /// </summary>
    public ICollection<UserRole> UserRoles { get; set; } = [];

    /// <summary>
    /// Active sessions (refresh tokens) for this user.
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}