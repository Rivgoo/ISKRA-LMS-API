using FluentValidation;
using Iskra.Modules.Auth.Password.Abstractions.Requests;
using Iskra.Modules.Validation.Abstractions.Options.Users;
using Microsoft.Extensions.Options;

namespace Iskra.Modules.Auth.Password.Validation;

internal class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator(
        IOptions<EmailValidationOptions> emailOptions,
        IOptions<PasswordValidationOptions> passwordOptions)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(emailOptions.Value.MaxLength);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MaximumLength(passwordOptions.Value.MaxLength);
    }
}