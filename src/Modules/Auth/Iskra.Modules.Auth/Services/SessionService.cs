using Iskra.Api.Abstractions.Models;
using Iskra.Application.Abstractions.Repositories;
using Iskra.Application.Errors;
using Iskra.Application.Results;
using Iskra.Core.Domain.Entities;
using Iskra.Modules.Auth.Abstractions.Errors;
using Iskra.Modules.Auth.Abstractions.Repositories;
using Iskra.Modules.Auth.Abstractions.Services;
using Iskra.Modules.Auth.Options;
using Iskra.Modules.Iam.Abstractions.Repositories;
using Iskra.Modules.Users.Abstractions.Repositories;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace Iskra.Modules.Auth.Services;

internal sealed class SessionService(
    IUserSessionRepository sessionRepository,
    IUserRepository userRepository,
    IUserRoleRepository userRoleRepository,
    IUnitOfWork unitOfWork,
    JwtProvider jwtProvider,
    ICookieService cookieService,
    IOptions<AuthOptions> authOptions)
    : ISessionService
{
    private readonly AuthOptions _options = authOptions.Value;

    public async Task<Result> StartSessionAsync(Guid userId, DeviceInfo device, CancellationToken ct = default)
    {
        // 1. Verify User Exists & Active
        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user is null) return EntityErrors<User, Guid>.NotFoundById(userId);
        if (!user.IsActive) return AuthErrors.UserInactive;

        if (_options.Security.RequireEmailConfirmation && !user.IsEmailConfirmed)
            return AuthErrors.EmailNotConfirmed;

        // 2. Generate Tokens
        var roles = await userRoleRepository.GetRolesForUserAsync(userId, ct);
        var roleNames = roles.Select(r => r.Name).ToList();

        var accessToken = jwtProvider.GenerateAccessToken(user, roleNames);

        // Raw token for the Client (Cookie)
        var rawRefreshToken = jwtProvider.GenerateRefreshTokenString();

        // Hashed token for the Database
        var refreshTokenHash = HashToken(rawRefreshToken);

        // 3. Create Session in DB (Store Hash)
        var session = new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RefreshTokenHash = refreshTokenHash,
            IpAddress = device.IpAddress,
            DeviceInfo = device.UserAgent,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(_options.Jwt.RefreshTokenExpirationDays),
            IsRevoked = false
        };

        sessionRepository.Add(session);
        await unitOfWork.SaveChangesAsync(ct);

        // 4. Set Cookies (Send Raw Token)
        cookieService.SetSessionCookies(accessToken, rawRefreshToken);

        return Result.Ok();
    }

    public async Task<Result> RefreshSessionAsync(string rawRefreshToken, DeviceInfo device, CancellationToken ct = default)
    {
        // 1. Hash the incoming raw token to find it in DB
        var tokenHash = HashToken(rawRefreshToken);

        // 2. Find Session by Hash
        var session = await sessionRepository.GetByHashAsync(tokenHash, ct);

        if (session is null || !session.IsActive)
        {
            // Security: If token is revoked but someone tries to use it -> Potential Theft!
            cookieService.DeleteSessionCookies();
            return AuthErrors.InvalidSession;
        }

        // 3. Rotate Refresh Token (Security Best Practice)
        // We revoke the OLD session (found by hash)
        await sessionRepository.RevokeAsync(session.Id, ct);

        // 4. Start a NEW session
        return await StartSessionAsync(session.UserId, device, ct);
    }

    public async Task<Result> TerminateCurrentSessionAsync(CancellationToken ct = default)
    {
        var rawToken = cookieService.GetRefreshToken();
        if (string.IsNullOrEmpty(rawToken)) return Result.Ok();

        // Hash to find in DB
        var tokenHash = HashToken(rawToken);

        var session = await sessionRepository.GetByHashAsync(tokenHash, ct);
        if (session != null)
        {
            await sessionRepository.RevokeAsync(session.Id, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }

        cookieService.DeleteSessionCookies();
        return Result.Ok();
    }

    public async Task<Result> TerminateAllOtherSessionsAsync(CancellationToken ct = default)
    {
        var rawToken = cookieService.GetRefreshToken();
        if (string.IsNullOrEmpty(rawToken)) return AuthErrors.MissingToken;

        // Hash to find current
        var tokenHash = HashToken(rawToken);

        var currentSession = await sessionRepository.GetByHashAsync(tokenHash, ct);
        if (currentSession == null) return AuthErrors.InvalidSession;

        await sessionRepository.RevokeAllExceptAsync(currentSession.UserId, currentSession.Id, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Ok();
    }

    /// <summary>
    /// Hashes the refresh token using SHA256.
    /// Fast and secure enough for high-entropy tokens.
    /// </summary>
    private static string HashToken(string rawToken)
    {
        var bytes = Encoding.UTF8.GetBytes(rawToken);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToBase64String(hashBytes);
    }
}