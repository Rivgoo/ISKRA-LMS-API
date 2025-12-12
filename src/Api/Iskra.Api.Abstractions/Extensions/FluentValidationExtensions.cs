using Iskra.Application.Errors;

using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Iskra.Api.Abstractions.Extensions;

public static class FluentValidationExtensions
{
    public static ValidationError ToError(this ValidationResult result)
    {
        var errors = result.Errors
            .Select(x => Error.Validation(x.PropertyName, x.ErrorMessage))
            .ToArray();

        return new ValidationError(errors);
    }
}