using Asp.Versioning;
using Iskra.Api.Abstractions.Extensions;
using Iskra.Application.Results;
using Iskra.Modules.Auth.Abstractions.Errors;
using Iskra.Modules.Auth.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Iskra.Modules.Auth.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
[Produces("application/json")]
[EnableRateLimiting("AuthPolicy")]
public class AuthController(
    ISessionService sessionService,
    ICookieService cookieService) : ControllerBase
{
    /// <summary>
    /// Refreshes the User Session using the Refresh Token from cookies.
    /// </summary>
    /// <remarks>
    /// This endpoint performs **Token Rotation**. 
    /// The old Refresh Token is revoked, and a new pair (Access + Refresh) is issued via Set-Cookie headers.
    /// Does not require the user to be authenticated via JWT (as the JWT might be expired).
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Session successfully refreshed. New cookies set.</response>
    /// <response code="401">Refresh token is missing, invalid, expired, or revoked.</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        // 1. Validate Input (Cookie Presence)
        var refreshToken = cookieService.GetRefreshToken();

        if (string.IsNullOrWhiteSpace(refreshToken))
            return Result.Bad(AuthErrors.MissingToken).ToActionResult();

        // 2. Extract Context Data
        var deviceInfo = HttpContext.GetDeviceInfo();

        // 3. Execute Logic
        var result = await sessionService.RefreshSessionAsync(refreshToken, deviceInfo, cancellationToken);

        if (result.IsFailure)
            return result.ToActionResult();

        return Ok(new { message = "Session refreshed successfully." });
    }

    /// <summary>
    /// Terminates the current session (Log Out).
    /// </summary>
    /// <remarks>
    /// Revokes the Refresh Token in the database and clears the auth cookies.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Logout successful (or session was already invalid).</response>
    [HttpPost("/logout")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await sessionService.TerminateCurrentSessionAsync(cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Revokes all active sessions for the user EXCEPT the current one.
    /// </summary>
    /// <remarks>
    /// Useful for "Sign out of all other devices" security feature.
    /// Requires a valid, active Access Token.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">All other sessions revoked.</response>
    /// <response code="401">User is not authenticated or session is invalid.</response>
    [HttpPost("revoke-all-others")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RevokeAllOthers(CancellationToken cancellationToken)
    {
        var result = await sessionService.TerminateAllOtherSessionsAsync(cancellationToken);

        return result.ToActionResult();
    }
}