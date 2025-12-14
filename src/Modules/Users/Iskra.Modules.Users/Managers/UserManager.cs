using Iskra.Application.Abstractions.Security;
using Iskra.Application.Errors.DomainErrors;
using Iskra.Application.Results;
using Iskra.Core.Domain.Entities;
using Iskra.Modules.Iam.Abstractions.Services;
using Iskra.Modules.Users.Abstractions.Models;
using Iskra.Modules.Users.Abstractions.Services;
using Iskra.Modules.Users.Options;
using Iskra.Modules.Validation.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Iskra.Modules.Users.Managers;

internal sealed class UserManager(
    IUserEntityService userEntityService,
    IUserRoleService userRoleService,
    IPasswordHasher passwordHasher,
    IPasswordValidationService passwordValidator,
    IOptions<UserRegistrationOptions> registrationOptions,
    ILogger<UserManager> logger)
    : IUserManager
{
    private readonly UserRegistrationOptions _options = registrationOptions.Value;

    public async Task<Result<Guid>> RegisterUserAsync(RegisterUserRequest request, CancellationToken ct = default)
    {
        // 1. Validate Mandatory Role
        if (string.IsNullOrWhiteSpace(request.Role))
            return Result<Guid>.Bad(UserErrors.RoleRequired);

        // 2. Password Check (Policy & Complexity)
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            if (!_options.AllowPasswordlessRegistration)
                return Result<Guid>.Bad(UserErrors.PasswordRequired);
        }
        else
        {
            // Validate Complexity
            var complexityResult = passwordValidator.Validate(request.Password);
            if (complexityResult.IsFailure)
                return Result<Guid>.Bad(complexityResult.Error!);
        }

        // 3. Check Uniqueness
        var existing = await userEntityService.GetByEmailAsync(request.Email, ct);
        if (existing.IsSuccess)
            return Result<Guid>.Bad(UserErrors.EmailAlreadyExists(request.Email));

        // 4. Hash Password
        var passwordHash = string.IsNullOrWhiteSpace(request.Password)
            ? string.Empty
            : passwordHasher.Hash(request.Password);

        // 5. Create Entity
        var newUser = User.Create(
            request.Email,
            request.FirstName,
            request.LastName,
            request.MiddleName,
            passwordHash,
            request.IsEmailConfirmed
        );

        var createResult = await userEntityService.CreateAsync(newUser, ct);
        if (createResult.IsFailure)
            return Result<Guid>.Bad(createResult.Error!);

        // 6. Assign Role (Critical Step)
        var roleResult = await userRoleService.AssignRoleAsync(newUser.Id, request.Role, ct);

        if (roleResult.IsFailure)
        {
            logger.LogError("Failed to assign role '{Role}' to user '{UserId}'. Rolling back creation. Error: {Error}",
                request.Role, newUser.Id, roleResult.Error?.Code);

            await userEntityService.DeleteByIdAsync(newUser.Id, ct);

            return Result<Guid>.Bad(roleResult.Error!);
        }

        return Result<Guid>.Ok(newUser.Id);
    }

    public async Task<Result> UpdateProfileAsync(Guid userId, UpdateUserRequest request, CancellationToken ct = default)
    {
        var userResult = await userEntityService.GetByIdAsync(userId, ct);
        if (userResult.IsFailure) return userResult.ToValue<Result>();

        var user = userResult.Value!;
        user.UpdateProfile(request.FirstName, request.LastName, request.MiddleName);

        return await userEntityService.UpdateAsync(user, ct);
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct = default)
    {
        var userResult = await userEntityService.GetByIdAsync(userId, ct);
        if (userResult.IsFailure) return userResult.ToValue<Result>();

        var user = userResult.Value!;

        // Verify Old Password
        if (!passwordHasher.Verify(request.OldPassword, user.PasswordHash))
            return UserErrors.InvalidOldPassword;

        // Validate New Password Complexity
        var complexityResult = passwordValidator.Validate(request.NewPassword);
        if (complexityResult.IsFailure)
            return complexityResult;

        // Hash New Password
        var newHash = passwordHasher.Hash(request.NewPassword);

        return await userEntityService.UpdatePasswordHashAsync(userId, newHash, ct);
    }

    public async Task<Result> AssignRoleAsync(Guid userId, string roleName, CancellationToken ct = default)
    {
        return await userRoleService.AssignRoleAsync(userId, roleName, ct);
    }

    public async Task<Result> RevokeRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        return await userRoleService.RevokeRoleAsync(userId, roleName, cancellationToken);
    }

    public async Task<Result> DeleteUserAsync(Guid userId, CancellationToken ct = default)
    {
        var exists = await userEntityService.CheckExistsByIdAsync(userId, ct);
        if (exists.IsFailure) return exists;

        return await userEntityService.DeleteByIdAsync(userId, ct);
    }
}