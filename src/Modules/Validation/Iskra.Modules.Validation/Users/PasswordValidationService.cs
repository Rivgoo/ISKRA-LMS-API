using Iskra.Application.Errors;
using Iskra.Application.Results;
using Iskra.Modules.Validation.Abstractions.Options.Users;
using Iskra.Modules.Validation.Abstractions.Services;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Iskra.Modules.Validation.Users;

internal class PasswordValidationService(IOptions<PasswordValidationOptions> options) : IPasswordValidationService
{
    private readonly PasswordValidationOptions _settings = options.Value;

    public Result Validate(string password)
    {
        if (string.IsNullOrEmpty(password))
            return ValidationErrors.IsRequired("Password");

        if (password.Length < _settings.MinLength)
            return PasswordErrors.TooShort(_settings.MinLength);

        if (password.Length > _settings.MaxLength)
            return PasswordErrors.TooLong(_settings.MaxLength);

        // Regex Override
        if (!string.IsNullOrWhiteSpace(_settings.CustomRegexPattern))
        {
            if (!Regex.IsMatch(password, _settings.CustomRegexPattern))
                return PasswordErrors.InvalidFormat;

            return Result.Ok();
        }

        // Flag Checks
        if (_settings.RequireDigit && !password.Any(char.IsDigit))
            return PasswordErrors.RequiresDigit;

        if (_settings.RequireLowercase && !password.Any(char.IsLower))
            return PasswordErrors.RequiresLower;

        if (_settings.RequireUppercase && !password.Any(char.IsUpper))
            return PasswordErrors.RequiresUpper;

        if (_settings.RequireNonAlphanumeric && password.All(char.IsLetterOrDigit))
            return PasswordErrors.RequiresNonAlphanumeric;

        return Result.Ok();
    }
}