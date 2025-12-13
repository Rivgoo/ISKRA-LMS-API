using FluentValidation;
using Iskra.Modules.Iam.Abstractions.Models;

namespace Iskra.Modules.Iam.Validation;

public class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
{
    public UpdateRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .Matches("^[a-zA-Z0-9_.-]+$");

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}