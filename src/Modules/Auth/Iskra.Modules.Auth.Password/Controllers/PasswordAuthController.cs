using Asp.Versioning;
using FluentValidation;
using Iskra.Api.Abstractions.Extensions;
using Iskra.Application.Results;
using Iskra.Modules.Auth.Password.Abstractions.Requests;
using Iskra.Modules.Auth.Password.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Iskra.Modules.Auth.Password.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
[Produces("application/json")]
[EnableRateLimiting("AuthPolicy")]
public class PasswordAuthController(
    IPasswordLoginService loginService,
    IValidator<LoginRequest> validator) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(request, ct);

        if (!validation.IsValid)
            return Result.Bad(validation.ToError()).ToActionResult();

        var deviceInfo = HttpContext.GetDeviceInfo();
        var result = await loginService.LoginAsync(request, deviceInfo, ct);

        return result.ToActionResult();
    }
}