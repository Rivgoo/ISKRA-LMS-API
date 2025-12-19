using FluentValidation;
using Iskra.Modules.Users.Abstractions.Models.Requests;
using Iskra.Modules.Validation.Abstractions.Options.Users;
using Microsoft.Extensions.Options;

namespace Iskra.Modules.Users.Validation;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator(IOptions<PasswordValidationOptions> passwordOptions)
    {
        var settings = passwordOptions.Value;

        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(settings.MinLength)
            .MaximumLength(settings.MaxLength)
            .NotEqual(x => x.OldPassword).WithMessage("New password must be different from the old password.");
    }
}