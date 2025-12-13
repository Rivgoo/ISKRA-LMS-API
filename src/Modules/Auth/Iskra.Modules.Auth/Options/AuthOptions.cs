namespace Iskra.Modules.Auth.Options;

internal class AuthOptions
{
    public JwtOptions Jwt { get; set; } = new();
    public CookieSettings Cookies { get; set; } = new();
    public SessionCleanupOptions SessionCleanup { get; set; } = new();
    public SecuritySettings Security { get; set; } = new();
}