using FluentValidation;
using Iskra.Modules.Users.Abstractions.Models.Requests;
using Iskra.Modules.Validation.Abstractions.Options.Users;
using Microsoft.Extensions.Options;

namespace Iskra.Modules.Users.Validation;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator(IOptions<UserValidationOptions> userOptions)
    {
        var settings = userOptions.Value;

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(settings.MinFirstNameLength, settings.MaxFirstNameLength);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(settings.MinLastNameLength, settings.MaxLastNameLength);

        RuleFor(x => x.MiddleName)
            .Length(settings.MinMiddleNameLength, settings.MaxMiddleNameLength)
            .When(x => !string.IsNullOrEmpty(x.MiddleName));
    }
}