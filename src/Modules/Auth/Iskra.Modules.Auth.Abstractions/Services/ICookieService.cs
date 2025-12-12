namespace Iskra.Modules.Auth.Abstractions.Services;

public interface ICookieService
{
    void SetSessionCookies(string accessToken, string refreshToken);
    void DeleteSessionCookies();
    string? GetRefreshToken();
}