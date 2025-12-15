namespace Iskra.Api.Abstractions.Responses;

/// <summary>
/// Represents a response that contains a single identifier value of the specified type.
/// </summary>
/// <typeparam name="T">The type of the identifier value contained in the response.</typeparam>
/// <param name="Id">The identifier value to include in the response.</param>
public record IdResponse<T>(T? Id);