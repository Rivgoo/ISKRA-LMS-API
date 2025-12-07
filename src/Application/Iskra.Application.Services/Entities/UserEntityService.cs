using Iskra.Application.Abstractions.Repositories;
using Iskra.Application.Abstractions.Repositories.Base;
using Iskra.Application.Abstractions.Services.Entities;
using Iskra.Application.Abstractions.Validation;
using Iskra.Application.Services.Base;
using Iskra.Core.Domain.Entities;

namespace Iskra.Application.Services.Entities;

/// <summary>
/// Implements the basic, validated CRUD service for User entities.
/// </summary>
internal class UserEntityService(
    IUserRepository repository,
    IUnitOfWork unitOfWork,
    IValidationService<User> validationService)
    : EntityService<User, Guid, IUserRepository>(repository, unitOfWork, validationService), IUserEntityService
{
}