using Asp.Versioning;
using FluentValidation;
using Iskra.Api.Abstractions.Authorization;
using Iskra.Api.Abstractions.Extensions;
using Iskra.Api.Abstractions.Filters;
using Iskra.Api.Abstractions.Responses;
using Iskra.Api.Abstractions.Services;
using Iskra.Application.Errors.DomainErrors;
using Iskra.Application.Filters.Abstractions;
using Iskra.Application.Results;
using Iskra.Core.Domain.Entities;
using Iskra.Modules.Auth.Abstractions.Errors;
using Iskra.Modules.Iam.Abstractions;
using Iskra.Modules.Users.Abstractions.Models.Requests;
using Iskra.Modules.Users.Abstractions.Models.Responses;
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
    IValidator<ChangePasswordRequest> passwordValidator,
    IDataQueryService<User, UserQueryRequest> queryService)
    : ControllerBase
{
    /// <summary>
    /// Searches and sorts users based on specific criteria.
    /// </summary>
    /// <param name="request">Filter criteria.</param>
    /// <param name="pageSize">Items per page.</param>
    /// <param name="sortParams">Sorting parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated list of users.</returns>
    [HttpGet("filter")]
    [HasPermission(IskraPermissions.Users.ReadDetail)]
    public async Task<IActionResult> GetByFilter(
        [FromQuery] UserQueryRequest request,
        [FromQuery] int pageSize,
        [FromQuery] SortParams sortParams,
        CancellationToken ct)
    {
        var sortResult = request.ApplySorts(sortParams);
        if (sortResult.IsFailure)
            return sortResult.ToActionResult();

        var result = await queryService
            .Prepare(request)
            .SetPageSize(pageSize)
            .ExecuteAsync<UserResponse>(ct);

        return result.ToActionResult();
    }

    /// <summary>
    /// Retrieves comprehensive profile data.
    /// </summary>
    /// <param name="userId">The target user ID. If null, returns current user data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A detailed user record including roles and system metadata.</returns>
    /// <response code="200">Returns the requested profile.</response>
    /// <response code="403">Returned if accessing another user profile without the 'users.read.detail' permission.</response>
    /// <response code="404">Returned if the user profile does not exist.</response>
    /// <response code="401">Returned if the request is not authenticated.</response>
    [HttpGet("{userId:guid?}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetFull(Guid? userId, CancellationToken ct)
    {
        var isExternalRequest = userId.HasValue && userId.Value != currentUser.Id;

        if (isExternalRequest)
        {
            var allowed = await currentUser.HasPermissionAsync(IskraPermissions.Users.ReadDetail);

            if (!allowed)
                return Result.Bad(AuthErrors.Forbidden).ToActionResult();
        }

        var result = await userManager.GetFullProfileAsync(userId, ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Retrieves public profile data.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Publicly accessible user information.</returns>
    /// <response code="200">Returns the public profile.</response>
    /// <response code="404">Returned if the user profile does not exist.</response>
    /// <response code="401">Returned if the request is not authenticated.</response>
    [HttpGet("public/{userId:guid?}")]
    [HasPermission(IskraPermissions.Users.Read)]
    [ProducesResponseType(typeof(UserPublicResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPublic(Guid? userId, CancellationToken ct)
    {
        var result = await userManager.GetPublicProfileAsync(userId, ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Creates a new user (Admin function).
    /// To register publicly, use the Public Registration endpoint.
    /// </summary>
    [HttpPost]
    [HasPermission(IskraPermissions.Users.Create)]
    [ProducesResponseType(typeof(IdResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] RegisterUserRequest request, CancellationToken ct)
    {
        var validation = await registerValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result.Bad(validation.ToError()).ToActionResult();

        var result = await userManager.RegisterUserAsync(request, ct);

        return result.Match(id =>
            CreatedAtAction(
                nameof(UpdateProfile),
                new { userId = id, version = "1" },
                new IdResponse<Guid>(id))
        );
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(Guid userId, CancellationToken ct)
    {
        if (currentUser.Id == userId)
            return Result.Bad(UserErrors.CannotDeleteSelf).ToActionResult();

        var result = await userManager.DeleteUserAsync(userId, ct);
        return result.ToActionResult();
    }
}