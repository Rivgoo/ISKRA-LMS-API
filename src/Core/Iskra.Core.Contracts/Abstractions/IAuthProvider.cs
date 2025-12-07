namespace Iskra.Core.Contracts.Abstractions;

/// <summary>
/// Defines the contract for authentication and credential management.
/// This allows swapping auth implementations (e.g., JWT vs. OAuth) without changing the core.
/// </summary>
public interface IAuthProvider
{
    /// <summary>
    /// Generates a secure token for a validated user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="role">The user's role.</param>
    /// <returns>A string representation of the token (e.g., JWT).</returns>
    string GenerateToken(Guid userId, string role);

    /// <summary>
    /// Validates an incoming token and extracts identity information.
    /// </summary>
    /// <param name="token">The token string to validate.</param>
    /// <param name="userId">The extracted user ID.</param>
    /// <param name="role">The extracted user role.</param>
    /// <returns>True if the token is valid; otherwise, false.</returns>
    bool ValidateToken(string token, out Guid userId, out string role);
}