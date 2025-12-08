using Iskra.Application.Results;
using Iskra.Core.Domain.Common;

namespace Iskra.Application.Abstractions.Validation.Base;

/// <summary>
/// Defines a contract for validating an entity against a set of rules.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to validate.</typeparam>
public interface IValidationService<in TEntity> where TEntity : IEntity
{
    /// <summary>
    /// Asynchronously validates an entity.
    /// </summary>
    /// <param name="entity">The entity to validate.</param>
    /// <returns>A Result indicating success or containing a collection of validation errors.</returns>
    Task<Result> ValidateAsync(TEntity entity);
}