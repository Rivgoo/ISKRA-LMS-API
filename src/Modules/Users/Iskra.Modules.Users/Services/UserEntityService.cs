using Iskra.Application.Abstractions.Repositories;
using Iskra.Application.Abstractions.Validation;
using Iskra.Application.Errors;
using Iskra.Application.Errors.DomainErrors;
using Iskra.Application.Results;
using Iskra.Application.Services;
using Iskra.Core.Domain.Entities;
using Iskra.Modules.Users.Abstractions.Repositories;
using Iskra.Modules.Users.Abstractions.Services;

namespace Iskra.Modules.Users.Services;

internal class UserEntityService(
    IUserRepository repository,
    IUnitOfWork unitOfWork,
    IValidationService<User> validationService)
    : EntityService<User, Guid, IUserRepository>(repository, unitOfWork, validationService), IUserEntityService
{
    public async Task<Result<User>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return ValidationErrors.IsRequired(nameof(User.Email));

        var user = await Repository.GetByEmailAsync(email, cancellationToken);

        if (user is null)
            return UserErrors.NotFoundByEmail(email);

        return Result<User>.Ok(user);
    }

    public async Task<Result> UpdatePasswordHashAsync(Guid userId, string newPasswordHash, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            return ValidationErrors.IsRequired(nameof(User.PasswordHash));

        var exists = await CheckExistsByIdAsync(userId, cancellationToken);
        if (exists.IsFailure) return exists;

        var rowsAffected = await Repository.UpdatePasswordHashAsync(userId, newPasswordHash, cancellationToken);

        return rowsAffected > 0
            ? Result.Ok()
            : Result.Bad(UserErrors.UpdatePasswordFailed);
    }

    public async Task<Result> SetActiveStatusAsync(Guid userId, bool isActive, CancellationToken cancellationToken = default)
    {
        var exists = await CheckExistsByIdAsync(userId, cancellationToken);
        if (exists.IsFailure) return exists;

        var rowsAffected = await Repository.SetActiveStatusAsync(userId, isActive, cancellationToken);

        return rowsAffected > 0
            ? Result.Ok()
            : Result.Bad(UserErrors.SetActiveStatusFailed);
    }
}