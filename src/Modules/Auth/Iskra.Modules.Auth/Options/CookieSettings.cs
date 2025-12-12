namespace Iskra.Modules.Auth.Options;

internal class CookieSettings
{
    /// <summary>
    /// Name of the cookie storing the JWT Access Token.
    /// Default: "iskra_access"
    /// </summary>
    public string AccessTokenName { get; set; } = "iskra_access";

    /// <summary>
    /// Name of the cookie storing the Refresh Token.
    /// Default: "iskra_refresh"
    /// </summary>
    public string RefreshTokenName { get; set; } = "iskra_refresh";

    /// <summary>
    /// SameSite mode for the cookies. 
    /// Valid values: "Strict", "Lax", "None".
    /// Recommended: "Strict" for high security API-only apps.
    /// </summary>
    public string SameSite { get; set; } = "Strict";

    /// <summary>
    /// If true, the cookie cannot be accessed by client-side JavaScript.
    /// Critical for XSS protection.
    /// </summary>
    public bool HttpOnly { get; set; } = true;

    /// <summary>
    /// If true, the cookie is only sent over HTTPS.
    /// </summary>
    public bool Secure { get; set; } = true;

    /// <summary>
    /// If true, cookies will have an Expiration date set (Persistent).
    /// If false, cookies will be Session-only (deleted on browser close).
    /// Default: true (Standard modern app behavior).
    /// </summary>
    public bool UsePersistentCookies { get; set; } = true;
}