using Iskra.Core.Domain.Common;

namespace Iskra.Core.Domain.Entities;

/// <summary>
/// Represents a registered user within the system.
/// </summary>
public class User : ConcurrentEntity<Guid>
{
    // Required by EF Core
    private User() { }

    /// <summary>
    /// Creates a new instance of the <see cref="User"/> class using a factory method.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="firstName">The user's first name.</param>
    /// <param name="lastName">The user's last name.</param>
    /// <param name="middleName">The user's middle name (optional).</param>
    /// <param name="passwordHash">The hashed password.</param>
    /// <param name="isEmailConfirmed">Initial email confirmation status.</param>
    /// <returns>A new valid User entity.</returns>
    public static User Create(string email, string firstName, string lastName, string? middleName, 
        string passwordHash, bool isEmailConfirmed)
    {
        Guard.NotNullOrWhiteSpace(email);
        Guard.NotNullOrWhiteSpace(firstName);
        Guard.NotNullOrWhiteSpace(lastName);

        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            MiddleName = middleName,
            PasswordHash = passwordHash,
            IsEmailConfirmed = isEmailConfirmed,
            IsActive = true,
            ConcurrencyToken = Guid.NewGuid()
        };
    }

    /// <summary>
    /// Gets the user's first name.
    /// </summary>
    public string FirstName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the user's last name.
    /// </summary>
    public string LastName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the user's middle name (optional).
    /// </summary>
    public string? MiddleName { get; private set; }

    /// <summary>
    /// Gets the unique email address.
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the BCrypt/Argon2 hash of the password.
    /// </summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the user is allowed to log in.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Gets a value indicating if the email address has been verified.
    /// </summary>
    public bool IsEmailConfirmed { get; private set; }

    /// <summary>
    /// Roles assigned to this user.
    /// </summary>
    public ICollection<UserRole> UserRoles { get; private set; } = [];

    /// <summary>
    /// Updates the user's profile information.
    /// </summary>
    public void UpdateProfile(string firstName, string lastName, string? middleName)
    {
        Guard.NotNullOrWhiteSpace(firstName);
        Guard.NotNullOrWhiteSpace(lastName);

        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
    }

    /// <summary>
    /// Updates the user's password hash.
    /// </summary>
    public void SetPasswordHash(string newHash)
    {
        Guard.NotNullOrWhiteSpace(newHash);
        PasswordHash = newHash;
    }

    /// <summary>
    /// Activates the user account.
    /// </summary>
    public void Activate() => IsActive = true;

    /// <summary>
    /// Deactivates the user account (ban/suspend).
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Marks the user's email as confirmed.
    /// </summary>
    public void ConfirmEmail() => IsEmailConfirmed = true;
}