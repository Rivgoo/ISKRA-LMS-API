using Iskra.Application.Results;

namespace Iskra.Application.Abstractions.Validation;

public interface IPasswordValidationService
{
    /// <summary>
    /// Validates a raw password string against security policies.
    /// </summary>
    Result Validate(string password);
}