namespace Iskra.Application.Errors;

/// <summary>
/// Contains generic validation error templates applicable to any entity.
/// </summary>
public static class ValidationErrors
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

    /// <summary>
    /// Creates an error indicating that the specified property has an invalid format.
    /// </summary>
    /// <param name="propertyName">The name of the property that failed validation due to an invalid format. Cannot be null or empty.</param>
    /// <returns>An <see cref="Error"/> representing a bad request error for an invalid property format.</returns>
    public static Error InvalidFormat(string propertyName)
        => Error.BadRequest("Validation.InvalidFormat", "Property '{0}' has an invalid format.", propertyName);

    /// <summary>
    /// Creates an error indicating that a property value does not belong to a valid domain.
    /// </summary>
    /// <param name="propertyName">The name of the property that failed domain validation. Cannot be null or empty.</param>
    /// <param name="domain">A description of the expected valid domain for the property. Cannot be null or empty.</param>
    /// <returns>An error representing a bad request due to an invalid domain value for the specified property.</returns>
    public static Error InvalidDomain(string propertyName, string domain)
        => Error.BadRequest("Validation.InvalidDomain",
            "Property '{0}' has no valid domain: {1}.", propertyName, domain);
}