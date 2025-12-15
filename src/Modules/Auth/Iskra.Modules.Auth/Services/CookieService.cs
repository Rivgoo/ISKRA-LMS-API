using Iskra.Modules.Auth.Abstractions.Services;
using Iskra.Modules.Auth.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Iskra.Modules.Auth.Services;

internal sealed class CookieService(
    IHttpContextAccessor httpContextAccessor,
    IOptions<AuthOptions> options,
    IHostEnvironment environment) : ICookieService
{
    private readonly AuthOptions _options = options.Value;

    public void SetSessionCookies(string accessToken, string refreshToken)
    {
        var context = httpContextAccessor.HttpContext;
        if (context is null) return;

        var settings = _options.Cookies;

        var isSecure = settings.Secure;

        if (environment.IsProduction() && !isSecure)
            throw new InvalidOperationException("In Production environment, cookies must be set as Secure.");

        var sameSite = Enum.Parse<SameSiteMode>(settings.SameSite);

        // 1. Access Token (Short Lived)
        AppendCookie(context, settings.AccessTokenName, accessToken, new CookieOptions
        {
            HttpOnly = settings.HttpOnly,
            Secure = settings.Secure,
            SameSite = sameSite,
            Expires = DateTime.UtcNow.AddMinutes(_options.Jwt.AccessTokenExpirationMinutes)
        });

        // 2. Refresh Token (Session or Persistent)
        var refreshOptions = new CookieOptions
        {
            HttpOnly = settings.HttpOnly,
            Secure = settings.Secure,
            SameSite = sameSite
        };

        if (settings.UsePersistentCookies)
            refreshOptions.Expires = DateTime.UtcNow.AddDays(_options.Jwt.RefreshTokenExpirationDays);

        AppendCookie(context, settings.RefreshTokenName, refreshToken, refreshOptions);
    }

    public void DeleteSessionCookies()
    {
        var context = httpContextAccessor.HttpContext;
        if (context is null) return;

        context.Response.Cookies.Delete(_options.Cookies.AccessTokenName);
        context.Response.Cookies.Delete(_options.Cookies.RefreshTokenName);
    }

    public string? GetRefreshToken()
    {
        return httpContextAccessor.HttpContext?.Request.Cookies[_options.Cookies.RefreshTokenName];
    }

    private static void AppendCookie(HttpContext context, string key, string value, CookieOptions options)
    {
        context.Response.Cookies.Append(key, value, options);
    }
}