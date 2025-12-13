using Asp.Versioning;
using FluentValidation;
using Iskra.Api.Abstractions.Authorization;
using Iskra.Api.Abstractions.Extensions;
using Iskra.Api.Abstractions.Services;
using Iskra.Application.Errors.DomainErrors;
using Iskra.Application.Results;
using Iskra.Modules.Iam.Abstractions;
using Iskra.Modules.Users.Abstractions.Models;
using Iskra.Modules.Users.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Iskra.Modules.Users.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
[Authorize]
public class UsersController(
    IUserManager userManager,
    ICurrentUser currentUser,
    IValidator<RegisterUserRequest> registerValidator,
    IValidator<UpdateUserRequest> updateValidator,
    IValidator<ChangePasswordRequest> passwordValidator)
    : ControllerBase
{
    /// <summary>
    /// Creates a new user (Admin function).
    /// To register publicly, use the Public Registration endpoint.
    /// </summary>
    [HttpPost]
    [HasPermission(IskraPermissions.Users.Create)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] RegisterUserRequest request, CancellationToken ct)
    {
        var validation = await registerValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result.Bad(validation.ToError()).ToActionResult();

        var result = await userManager.RegisterUserAsync(request, ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Updates user profile information.
    /// Access: Self or User with 'users.update' permission.
    /// </summary>
    [HttpPut("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile(Guid userId, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        if (!await currentUser.CanAccessAsync(userId, IskraPermissions.Users.Update))
            return Forbid();

        var validation = await updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result.Bad(validation.ToError()).ToActionResult();

        var result = await userManager.UpdateProfileAsync(userId, request, ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Changes the password for the specified user.
    /// Strict Access: Only the user themselves can change their password via this endpoint (requires old password).
    /// </summary>
    [HttpPost("{userId:guid}/change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ChangePassword(Guid userId, [FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        if (currentUser.Id != userId)
            return Forbid();

        var validation = await passwordValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result.Bad(validation.ToError()).ToActionResult();

        var result = await userManager.ChangePasswordAsync(userId, request, ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    [HttpPost("{userId:guid}/roles/{roleName}")]
    [HasPermission(IskraPermissions.Roles.ManageAssignments)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignRole(Guid userId, string roleName, CancellationToken ct)
    {
        var result = await userManager.AssignRoleAsync(userId, roleName, ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Revokes a role from a user.
    /// </summary>
    [HttpDelete("{userId:guid}/roles/{roleName}")]
    [HasPermission(IskraPermissions.Roles.ManageAssignments)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokeRole(Guid userId, string roleName, CancellationToken ct)
    {
        var result = await userManager.RevokeRoleAsync(userId, roleName, ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Permanently deletes a user and all associated data.
    /// Access: 'users.delete'. Users cannot delete themselves via this API.
    /// </summary>
    [HttpDelete("{userId:guid}")]
    [HasPermission(IskraPermissions.Users.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid userId, CancellationToken ct)
    {
        if (currentUser.Id == userId)
            return Result.Bad(UserErrors.CannotDeleteSelf).ToActionResult();

        var result = await userManager.DeleteUserAsync(userId, ct);
        return result.ToActionResult();
    }
}