using Iskra.Application.Results;

namespace Iskra.Modules.Validation.Abstractions.Services;

public interface IPasswordValidationService
{
    /// <summary>
    /// Validates a raw password string against security policies.
    /// </summary>
    Result Validate(string password);
}