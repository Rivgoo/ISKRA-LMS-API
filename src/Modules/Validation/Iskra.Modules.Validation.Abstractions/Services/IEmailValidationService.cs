using Iskra.Application.Results;

namespace Iskra.Modules.Validation.Abstractions.Services;

public interface IEmailValidationService
{
    /// <summary>
    /// Validates an email string format.
    /// </summary>
    Result Validate(string email);
}