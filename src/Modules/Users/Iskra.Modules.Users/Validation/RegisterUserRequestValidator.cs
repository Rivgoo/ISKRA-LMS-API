using FluentValidation;
using Iskra.Modules.Users.Abstractions.Models;
using Iskra.Modules.Validation.Abstractions.Options.Users;
using Microsoft.Extensions.Options;

namespace Iskra.Modules.Users.Validation;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator(
        IOptions<UserValidationOptions> userOptions,
        IOptions<EmailValidationOptions> emailOptions,
        IOptions<PasswordValidationOptions> passwordOptions)
    {
        var userSettings = userOptions.Value;
        var emailSettings = emailOptions.Value;
        var passwordSettings = passwordOptions.Value;

        // Email Validation
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(emailSettings.MaxLength);

        // First Name
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(userSettings.MinFirstNameLength, userSettings.MaxFirstNameLength);

        // Last Name
        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(userSettings.MinLastNameLength, userSettings.MaxLastNameLength);

        // Middle Name (Optional)
        RuleFor(x => x.MiddleName)
            .Length(userSettings.MinMiddleNameLength, userSettings.MaxMiddleNameLength)
            .When(x => !string.IsNullOrEmpty(x.MiddleName) || userSettings.MinMiddleNameLength > 0);

        // Role
        RuleFor(x => x.Role)
            .NotEmpty();

        // Password (Length Only - Complexity handled by Service)
        // We validate length here to fail fast before hitting the heavier service logic.
        RuleFor(x => x.Password)
            .MinimumLength(passwordSettings.MinLength)
            .MaximumLength(passwordSettings.MaxLength)
            .When(x => !string.IsNullOrEmpty(x.Password));
    }
}