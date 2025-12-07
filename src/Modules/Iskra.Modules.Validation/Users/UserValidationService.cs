using Iskra.Application.Abstractions.Validation;
using Iskra.Application.Results;
using Iskra.Core.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Iskra.Modules.Validation.Users;

internal class UserValidationService(IOptions<UserValidationOptions> options)
    : IValidationService<User>
{
    private readonly UserValidationOptions _options = options.Value;

    public Task<Result> ValidateAsync(User entity)
    {
        if (entity.FirstName.Length < _options.MinFirstNameLength)
        {
            return Task.FromResult(Result.Bad(
                EntityErrors.Validation.TooShort(nameof(User.FirstName), _options.MinFirstNameLength)));
        }

        if (string.IsNullOrWhiteSpace(entity.Email))
        {
            return Task.FromResult(Result.Bad(
                EntityErrors.Validation.IsRequired(nameof(User.Email))));
        }

        return Task.FromResult(Result.Ok());
    }
}