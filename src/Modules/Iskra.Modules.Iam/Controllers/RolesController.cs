using Asp.Versioning;
using FluentValidation;
using Iskra.Api.Abstractions.Authorization;
using Iskra.Api.Abstractions.Extensions;
using Iskra.Api.Abstractions.Responses;
using Iskra.Application.Results;
using Iskra.Modules.Iam.Abstractions;
using Iskra.Modules.Iam.Abstractions.Models;
using Iskra.Modules.Iam.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Iskra.Modules.Iam.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/iam/roles")]
[Authorize]
public class RolesController(
    IRoleManager roleManager,
    IValidator<CreateRoleRequest> createValidator,
    IValidator<UpdateRoleRequest> updateValidator,
    IValidator<UpdateRolePermissionsRequest> permissionsValidator)
    : ControllerBase
{
    /// <summary>
    /// Retrieves all roles in the system with user counts.
    /// </summary>
    [HttpGet]
    [HasPermission(IskraPermissions.Roles.Read)]
    [ProducesResponseType(typeof(List<RoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await roleManager.GetAllRolesAsync(ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Retrieves details of a specific role by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [HasPermission(IskraPermissions.Roles.Read)]
    [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await roleManager.GetRoleByIdAsync(id, ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Creates a new custom role.
    /// </summary>
    [HttpPost]
    [HasPermission(IskraPermissions.Roles.Create)]
    [ProducesResponseType(typeof(IdResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request, CancellationToken ct)
    {
        var validation = await createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result.Bad(validation.ToError()).ToActionResult();

        var result = await roleManager.CreateRoleAsync(request, ct);

        return result.Match(role =>
            CreatedAtAction(
                nameof(GetById),
                new { id = role.Id, version = "1" },
                new IdResponse<Guid>(role.Id))
        );
    }

    /// <summary>
    /// Updates an existing role's name or description.
    /// </summary>
    [HttpPut("{id:guid}")]
    [HasPermission(IskraPermissions.Roles.Update)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleRequest request, CancellationToken ct)
    {
        var validation = await updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result.Bad(validation.ToError()).ToActionResult();

        var result = await roleManager.UpdateRoleAsync(id, request, ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Deletes a role if it is not a system role and has no assigned users.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [HasPermission(IskraPermissions.Roles.Delete)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await roleManager.DeleteRoleAsync(id, ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Gets the list of permission strings assigned to a role.
    /// </summary>
    [HttpGet("{id:guid}/permissions")]
    [HasPermission(IskraPermissions.Roles.Read)]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPermissions(Guid id, CancellationToken ct)
    {
        var result = await roleManager.GetPermissionsAsync(id, ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Replaces the permissions of a role with a new set.
    /// </summary>
    [HttpPut("{id:guid}/permissions")]
    [HasPermission(IskraPermissions.Roles.ManageAssignments)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdatePermissions(Guid id, [FromBody] UpdateRolePermissionsRequest request, CancellationToken ct)
    {
        var validation = await permissionsValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result.Bad(validation.ToError()).ToActionResult();

        var result = await roleManager.UpdatePermissionsAsync(id, request.Permissions, ct);
        return result.ToActionResult();
    }

    /// <summary>
    /// Returns a list of all available system permission strings defined in the codebase.
    /// Used for populating UI checkboxes.
    /// </summary>
    [HttpGet("system-permissions")]
    [HasPermission(IskraPermissions.Roles.Read)]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetAvailablePermissions()
    {
        var perms = PermissionDiscovery.GetAllPermissions();
        return Ok(perms);
    }
}