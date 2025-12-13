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

namespace Iskra.Modules.Auth.Services;

internal sealed class SessionService(
    IUserSessionRepository sessionRepository,
    IUserRepository userRepository,
    IPermissionRepository permissionRepository,
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

        // 2. Generate Token with Roles
        var roles = await userRoleRepository.GetRolesForUserAsync(userId, ct);
        var roleNames = roles.Select(r => r.Name).ToList();

        var accessToken = jwtProvider.GenerateAccessToken(user, roleNames);
        var refreshTokenString = jwtProvider.GenerateRefreshTokenString();

        // 3. Create Session in DB
        var session = new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RefreshToken = refreshTokenString,
            IpAddress = device.IpAddress,
            DeviceInfo = device.UserAgent,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(_options.Jwt.RefreshTokenExpirationDays),
            IsRevoked = false
        };

        sessionRepository.Add(session);
        await unitOfWork.SaveChangesAsync(ct);

        // 4. Set Cookies
        cookieService.SetSessionCookies(accessToken, refreshTokenString);

        return Result.Ok();
    }

    public async Task<Result> RefreshSessionAsync(string refreshToken, DeviceInfo device, CancellationToken ct = default)
    {
        // 1. Find Session
        var session = await sessionRepository.GetByTokenAsync(refreshToken, ct);

        if (session is null || !session.IsActive)
        {
            // Security: If token is revoked but someone tries to use it -> Potential Theft!
            cookieService.DeleteSessionCookies();
            return AuthErrors.InvalidSession;
        }

        // 2. Rotate Refresh Token (Security Best Practice)
        await sessionRepository.RevokeAsync(session.Id, ct);

        return await StartSessionAsync(session.UserId, device, ct);
    }

    public async Task<Result> TerminateCurrentSessionAsync(CancellationToken ct = default)
    {
        var token = cookieService.GetRefreshToken();
        if (string.IsNullOrEmpty(token)) return Result.Ok(); // Already logged out

        var session = await sessionRepository.GetByTokenAsync(token, ct);
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
        var token = cookieService.GetRefreshToken();
        if (string.IsNullOrEmpty(token)) return AuthErrors.MissingToken;

        var currentSession = await sessionRepository.GetByTokenAsync(token, ct);
        if (currentSession == null) return AuthErrors.InvalidSession;

        await sessionRepository.RevokeAllExceptAsync(currentSession.UserId, currentSession.Id, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Ok();
    }
}