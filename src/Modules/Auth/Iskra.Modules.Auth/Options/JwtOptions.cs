namespace Iskra.Modules.Auth.Options;

internal class JwtOptions
{
    /// <summary>
    /// The secret key used to sign the JWT. Must be at least 32 characters long.
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// The issuer of the token (e.g., "Iskra.Api").
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// The expected audience of the token (e.g., "Iskra.Client").
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Lifespan of the Access Token in minutes. (Standard: 15-30 mins).
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 15;

    /// <summary>
    /// Lifespan of the Refresh Token (User Session) in days. (Standard: 7-30 days).
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;
}