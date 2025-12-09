using Iskra.Application.Abstractions.Validation;
using Iskra.Application.Errors;
using Iskra.Application.Results;
using Iskra.Core.Domain.Entities;
using Iskra.Modules.Validation.Abstractions.Services;
using Microsoft.Extensions.Options;

namespace Iskra.Modules.Validation.Users;

/// <summary>
/// Orchestrates validation for the User entity.
/// Uses IEmailValidationService for the email property.
/// </summary>
internal class UserValidationService(
    IOptions<UserValidationOptions> options,
    IEmailValidationService emailService)
    : IValidationService<User>
{
    private readonly UserValidationOptions _options = options.Value;

    public Task<Result> ValidateAsync(User entity)
    {
        if (entity is null)
            return Task.FromResult<Result>(EntityErrors<User, Guid>.CreateNullFailure);

        // 1. First Name
        if (string.IsNullOrWhiteSpace(entity.FirstName))
            return Task.FromResult<Result>(ValidationErrors.IsRequired(nameof(User.FirstName)));

        if (entity.FirstName.Length < _options.MinFirstNameLength)
            return Task.FromResult<Result>(ValidationErrors.TooShort(nameof(User.FirstName), _options.MinFirstNameLength));

        if (entity.FirstName.Length > _options.MaxFirstNameLength)
            return Task.FromResult<Result>(ValidationErrors.TooLong(nameof(User.FirstName), _options.MaxFirstNameLength));

        // 2. Last Name
        if (string.IsNullOrWhiteSpace(entity.LastName))
            return Task.FromResult<Result>(ValidationErrors.IsRequired(nameof(User.LastName)));

        if (entity.LastName.Length < _options.MinLastNameLength)
            return Task.FromResult<Result>(ValidationErrors.TooShort(nameof(User.LastName), _options.MinLastNameLength));

        if (entity.LastName.Length > _options.MaxLastNameLength)
            return Task.FromResult<Result>(ValidationErrors.TooLong(nameof(User.LastName), _options.MaxLastNameLength));

        // 3. Middle Name (Optional)
        if (!string.IsNullOrWhiteSpace(entity.MiddleName))
        {
            if (entity.MiddleName.Length < _options.MinMiddleNameLength)
                return Task.FromResult<Result>(ValidationErrors.TooShort(nameof(User.MiddleName), _options.MinMiddleNameLength));
            
            if (entity.MiddleName.Length > _options.MaxMiddleNameLength)
                return Task.FromResult<Result>(ValidationErrors.TooLong(nameof(User.MiddleName), _options.MaxMiddleNameLength));
        }

        // 4. Email (Delegated)
        var emailResult = emailService.Validate(entity.Email);
        if (emailResult.IsFailure)
            return Task.FromResult(emailResult);

        return Task.FromResult(Result.Ok());
    }
}