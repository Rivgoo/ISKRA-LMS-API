using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Iskra.Core.Domain.Common;

/// <summary>
/// A static helper class for protecting methods and constructors from invalid inputs.
/// Throws consistent exceptions to enforce domain invariants.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Ensures that the argument value is not null.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <exception cref="ArgumentNullException">Thrown if the value is null.</exception>
    public static void NotNull<T>([NotNull] T? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value is null)
            throw new ArgumentNullException(paramName);
    }

    /// <summary>
    /// Ensures that the string argument is not null, empty, or consisting only of white-space characters.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <exception cref="ArgumentException">Thrown if the string is null, empty, or whitespace.</exception>
    public static void NotNullOrWhiteSpace([NotNull] string? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"'{paramName}' cannot be null or empty.", paramName);
    }

    /// <summary>
    /// Ensures that the Guid argument is not empty.
    /// </summary>
    /// <param name="value">The Guid value to check.</param>
    /// <param name="paramName">The name of the parameter (automatically captured).</param>
    /// <exception cref="ArgumentException">Thrown if the Guid is empty.</exception>
    public static void NotEmpty(Guid value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value == Guid.Empty)
            throw new ArgumentException($"'{paramName}' cannot be an empty GUID.", paramName);
    }

    /// <summary>
    /// Ensures that the specified condition is true. 
    /// Used for asserting domain business rules/invariants.
    /// </summary>
    /// <param name="condition">The condition that must be true.</param>
    /// <param name="message">The exception message if the condition is false.</param>
    /// <exception cref="InvalidOperationException">Thrown if the condition is false.</exception>
    public static void IsTrue(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}