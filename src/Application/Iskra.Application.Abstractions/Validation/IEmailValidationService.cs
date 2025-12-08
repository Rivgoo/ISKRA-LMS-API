using Iskra.Application.Results;

namespace Iskra.Application.Abstractions.Validation;

public interface IEmailValidationService
{
    /// <summary>
    /// Validates an email string format.
    /// </summary>
    Result Validate(string email);
}