using Iskra.Core.Domain.Common;

namespace Iskra.Core.Domain.Entities;

/// <summary>
/// Represents an active security session for a user on a specific device.
/// Replaces the concept of a simple Refresh Token.
/// </summary>
public class UserSession : BaseEntity<Guid>
{
    /// <summary>
    /// The opaque refresh token string used to obtain new access tokens.
    /// Should be hashed or encrypted in a high-security environment, but raw is acceptable for now if DB is secure.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// When the refresh token expires.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// If true, this session was manually terminated (Log out or Ban).
    /// </summary>
    public bool IsRevoked { get; set; }

    /// <summary>
    /// The IP address from which the session was created.
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User Agent string or Device Identifier (e.g., "Chrome on Windows 11", "iPhone 14").
    /// </summary>
    public string? DeviceInfo { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? RevokedAt { get; set; }

    /// <summary>
    /// Determines if the session is currently valid for refreshing tokens.
    /// </summary>
    public bool IsActive => !IsRevoked && DateTimeOffset.UtcNow < ExpiresAt;
}