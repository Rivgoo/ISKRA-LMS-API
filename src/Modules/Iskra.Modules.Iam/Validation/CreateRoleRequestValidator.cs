using FluentValidation;
using Iskra.Modules.Iam.Abstractions.Models;

namespace Iskra.Modules.Iam.Validation;

public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role name is required.")
            .MaximumLength(100).WithMessage("Role name must not exceed 100 characters.")
            .Matches("^[a-zA-Z0-9_.-]+$").WithMessage("Role name can only contain letters, numbers, underscores, dots, and dashes.");

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.Permissions)
            .NotNull();
    }
}