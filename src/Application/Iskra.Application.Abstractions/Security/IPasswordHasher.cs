namespace Iskra.Application.Abstractions.Security;

/// <summary>
/// Provides cryptographic hashing and verification for passwords.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plain-text password using a secure algorithm (e.g., BCrypt/Argon2).
    /// </summary>
    /// <param name="password">The plain-text password.</param>
    /// <returns>The hashed password string containing the salt and algorithm version.</returns>
    string Hash(string password);

    /// <summary>
    /// Verifies a plain-text password against a stored hash.
    /// </summary>
    /// <param name="password">The plain-text password to check.</param>
    /// <param name="hash">The stored hash to compare against.</param>
    /// <returns>True if the password matches the hash; otherwise, false.</returns>
    bool Verify(string password, string hash);
}