using Iskra.Core.Domain.Common;

namespace Iskra.Application.Results;

/// <summary>
/// Provides common error templates specific to a domain entity with a generic identifier.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TId">The type of the entity's unique identifier.</typeparam>
/// <remarks>
/// Constrained to work with types implementing <see cref="IBaseEntity{TId}"/>.
/// Assumes the entity type represents a database table or aggregate root.
/// </remarks>
public class EntityErrors<TEntity, TId>
    where TEntity : IBaseEntity<TId>, IEntity
    where TId : notnull, IComparable<TId>
{
    /// <summary>
    /// Gets the name of the entity type.
    /// </summary>
    public static string EntityName => typeof(TEntity).Name;

    /// <summary>
    /// Creates an error indicating that the entity with the specified identifier was not found.
    /// </summary>
    /// <param name="id">The identifier of the entity that was not found.</param>
    /// <returns>A <see cref="Error"/> instance representing the not found error.</returns>
    public static Error NotFoundById(TId id)
        => Error.NotFound($"{EntityName}.NotFound", "{0} with Id '{1}' not found.", EntityName, id);
    
    /// <summary>
    /// Gets a predefined error instance that represents a 'not found' condition for the current entity type.
    /// </summary>
    /// <remarks>Use this property to indicate that an entity could not be located during an operation, such
    /// as a lookup or retrieval. The error message will include the entity name to provide context in error handling
    /// scenarios.</remarks>
    public static Error NotFound
        => Error.NotFound($"{EntityName}.NotFound", "{0} not found.", EntityName);

    /// <summary>
    /// Creates an error indicating that the entity object is null during a creation operation.
    /// </summary>
    /// <returns>A <see cref="Error"/> instance.</returns>
    public static Error CreateNullFailure
        => Error.Failure($"{EntityName}.CreateNullFailure", "Cannot create {0}: the provided entity object is null.", EntityName);

    /// <summary>
    /// Creates an error indicating that the entity object is null during an update operation.
    /// </summary>
    /// <returns>A <see cref="Error"/> instance.</returns>
    public static Error UpdateNullFailure
        => Error.Failure($"{EntityName}.UpdateNullFailure", "Cannot update {0}: the provided entity object is null.", EntityName);

    /// <summary>
    /// Creates an error indicating that the entity object is null during a delete operation.
    /// </summary>
    /// <returns>A <see cref="Error"/> instance.</returns>
    public static Error DeleteNullFailure
        => Error.Failure($"{EntityName}.DeleteNullFailure", "Cannot delete {0}: the provided entity object is null.", EntityName);

    /// <summary>
    /// Creates an error indicating a conflict due to a unique constraint violation.
    /// </summary>
    public static Error Conflict<TValue>(string propertyName, TValue conflictingValue) where TValue : notnull
       => Error.Conflict($"{EntityName}.Conflict",
           "{0} with {1} '{2}' already exists.", EntityName, propertyName, conflictingValue);
}

/// <summary>
/// Provides common error templates for domain entities.
/// </summary>
public static class EntityErrors
{
    /// <summary>
    /// Contains generic validation error templates applicable to any entity.
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// Creates an error for a value that is outside the allowed range.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        /// <returns>A validation error.</returns>
        public static Error OutOfRange<T>(string propertyName, T min, T max) where T : IComparable<T>
            => Error.BadRequest("Validation.OutOfRange",
                "Property '{0}' must be between {1} and {2}.", propertyName, min, max);

        /// <summary>
        /// Creates an error for a value that is below the minimum allowed value.
        /// </summary>
        public static Error TooLow<T>(string propertyName, T min) where T : IComparable<T>
            => Error.BadRequest("Validation.TooLow",
                "Property '{0}' must be at least {1}.", propertyName, min);

        /// <summary>
        /// Creates an error for a value that exceeds the maximum allowed value.
        /// </summary>
        public static Error TooHigh<T>(string propertyName, T max) where T : IComparable<T>
            => Error.BadRequest("Validation.TooHigh",
                "Property '{0}' must not be greater than {1}.", propertyName, max);

        /// <summary>
        /// Creates an error for a string that is shorter than the minimum required length.
        /// </summary>
        public static Error TooShort(string propertyName, int minLength)
            => Error.BadRequest("Validation.TooShort",
                "Property '{0}' must be at least {1} characters long.", propertyName, minLength);

        /// <summary>
        /// Creates an error for a string that is longer than the maximum allowed length.
        /// </summary>
        public static Error TooLong(string propertyName, int maxLength)
            => Error.BadRequest("Validation.TooLong",
                "Property '{0}' must not exceed {1} characters.", propertyName, maxLength);

        /// <summary>
        /// Creates an error for a required property that is null or empty.
        /// </summary>
        public static Error IsRequired(string propertyName)
            => Error.BadRequest("Validation.IsRequired",
                "Property '{0}' is required.", propertyName);

        /// <summary>
        /// Creates an error for a value that has an invalid format.
        /// </summary>
        public static Error InvalidFormat(string propertyName, string expectedFormat)
            => Error.BadRequest("Validation.InvalidFormat",
                "Property '{0}' has an invalid format. Expected format: {1}.", propertyName, expectedFormat);
    }
}