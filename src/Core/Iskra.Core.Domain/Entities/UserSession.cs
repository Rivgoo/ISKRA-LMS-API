using Iskra.Core.Domain.Common;

namespace Iskra.Core.Domain.Entities;

/// <summary>
/// Represents an active security session for a user on a specific device.
/// Replaces the concept of a simple Refresh Token.
/// </summary>
public class UserSession : BaseEntity<Guid>
{
    // Required by EF Core
    private UserSession() { }

    /// <summary>
    /// Creates a new UserSession.
    /// </summary>
    /// <param name="userId">The ID of the user owning the session.</param>
    /// <param name="tokenHash">The hash of the refresh token.</param>
    /// <param name="ipAddress">The client IP address.</param>
    /// <param name="deviceInfo">The user agent or device string.</param>
    /// <param name="expirationDays">Number of days until the session expires.</param>
    /// <returns>A new UserSession entity.</returns>
    public static UserSession Create(Guid userId, string tokenHash, string? ipAddress, 
        string? deviceInfo, int expirationDays)
    {
        Guard.NotEmpty(userId);
        Guard.NotNullOrWhiteSpace(tokenHash);

        return new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RefreshTokenHash = tokenHash,
            IpAddress = ipAddress,
            DeviceInfo = deviceInfo,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(expirationDays),
            CreatedAt = DateTimeOffset.UtcNow,
            IsRevoked = false
        };
    }

    /// <summary>
    /// The HASH of the refresh token.
    /// </summary>
    public string RefreshTokenHash { get; private set; } = string.Empty;

    /// <summary>
    /// When the refresh token expires.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; private set; }

    /// <summary>
    /// If true, this session was manually terminated (Log out or Ban).
    /// </summary>
    public bool IsRevoked { get; private set; }

    /// <summary>
    /// The IP address from which the session was created.
    /// </summary>
    public string? IpAddress { get; private set; }

    /// <summary>
    /// User Agent string or Device Identifier.
    /// </summary>
    public string? DeviceInfo { get; private set; }

    public Guid UserId { get; private set; }
    public User? User { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }

    /// <summary>
    /// Determines if the session is currently valid for refreshing tokens.
    /// </summary>
    public bool IsActive => !IsRevoked && DateTimeOffset.UtcNow < ExpiresAt;

    /// <summary>
    /// Marks the session as revoked (terminated).
    /// </summary>
    public void Revoke()
    {
        if (IsRevoked) return;

        IsRevoked = true;
        RevokedAt = DateTimeOffset.UtcNow;
    }
}