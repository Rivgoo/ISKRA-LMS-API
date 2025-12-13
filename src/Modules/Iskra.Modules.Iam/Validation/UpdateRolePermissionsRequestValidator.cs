using FluentValidation;
using Iskra.Modules.Iam.Abstractions.Models;

namespace Iskra.Modules.Iam.Validation;

public class UpdateRolePermissionsRequestValidator : AbstractValidator<UpdateRolePermissionsRequest>
{
    public UpdateRolePermissionsRequestValidator()
    {
        RuleFor(x => x.Permissions)
            .NotNull()
            .WithMessage("Permissions list cannot be null.");
    }
}