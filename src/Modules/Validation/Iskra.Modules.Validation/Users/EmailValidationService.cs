using Iskra.Application.Errors;
using Iskra.Application.Results;
using Iskra.Modules.Validation.Abstractions.Options.Users;
using Iskra.Modules.Validation.Abstractions.Services;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Iskra.Modules.Validation.Users;

internal class EmailValidationService(IOptions<EmailValidationOptions> options) : IEmailValidationService
{
    private readonly EmailValidationOptions _options = options.Value;

    public Result Validate(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return ValidationErrors.IsRequired("Email");

        if (!Regex.IsMatch(email, _options.RegexPattern))
            return ValidationErrors.InvalidFormat("Email");

        if (email.Length > _options.MaxLength)
            return ValidationErrors.TooLong("Email", _options.MaxLength);

        if(_options.AllowedDomains.Count == 0)
            return Result.Ok();

        var domain = email.Split('@')[1];
        if (!_options.AllowedDomains.Contains(domain))
            return ValidationErrors.InvalidDomain("Email", domain);

        return Result.Ok();
    }
}