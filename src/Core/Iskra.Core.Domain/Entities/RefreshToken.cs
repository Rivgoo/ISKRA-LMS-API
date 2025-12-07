using Iskra.Core.Domain.Common;

namespace Iskra.Core.Domain.Entities;

/// <summary>
/// Represents a security token used to refresh JWTs without re-entering credentials.
/// </summary>
public class RefreshToken : BaseEntity<Guid>
{
    /// <summary>
    /// The actual token string.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// The expiration date (UTC).
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Indicates if the token has been manually revoked (logged out).
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// The IP address that created this token (security audit).
    /// </summary>
    public string? CreatedByIp { get; set; }

    /// <summary>
    /// The user who owns this token.
    /// </summary>
    public Guid UserId { get; set; }
    public User? User { get; set; }

    /// <summary>
    /// Determines if the token is currently valid (not expired and not revoked).
    /// </summary>
    public bool IsActive => !IsRevoked && DateTimeOffset.UtcNow < ExpiresAt;
}