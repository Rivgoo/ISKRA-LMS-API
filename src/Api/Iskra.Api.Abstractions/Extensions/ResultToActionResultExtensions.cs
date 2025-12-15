using Iskra.Api.Abstractions.Responses;
using Iskra.Application.Errors;
using Iskra.Application.Results;
using Microsoft.AspNetCore.Mvc;

namespace Iskra.Api.Abstractions.Extensions;

/// <summary>
/// Provides extension methods to convert Result objects into ASP.NET Core ActionResults.
/// </summary>
public static class ResultToActionResultExtensions
{
    /// <summary>
    /// Converts a Result to IActionResult (200 OK or Error Object).
    /// </summary>
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
            return new OkResult();

        return result.ToErrorActionResult();
    }

    /// <summary>
    /// Converts a Result&lt;T&gt; to IActionResult (200 OK with Value or Error Object).
    /// </summary>
    public static IActionResult ToActionResult<TValue>(this Result<TValue> result)
    {
        if (result.IsSuccess)
        {
            return result.Value is not null
                ? new OkObjectResult(result.Value)
                : new OkResult();
        }

        return result.ToErrorActionResult();
    }

    /// <summary>
    /// Private helper to standardize error response structure.
    /// </summary>
    private static ObjectResult ToErrorActionResult(this Result result)
    {
        var error = result.Error ?? Error.Failure("Unknown.Error", "An unknown error occurred.");

        var problemDetails = new ProblemDetails
        {
            Status = (int)error.ToHttpStatusCode(),
            Title = error.Code,
            Detail = error.Description,
            Type = error.ErrorType.ToString()
        };

        if (error is ValidationError validationError)
        {
            var groupedErrors = validationError.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.Description).ToArray()
                );

            problemDetails.Extensions.Add("errors", groupedErrors);
        }

        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status,
            ContentTypes = { "application/problem+json" }
        };
    }

    /// <summary>
    /// Matches the result. If success, executes the success function to return an IActionResult (usually Created).
    /// If failure, returns the standard error ActionResult.
    /// </summary>
    public static IActionResult Match<T>(
        this Result<T> result,
        Func<T, IActionResult> onSuccess)
    {
        if (result.IsFailure)
            return result.ToErrorActionResult();

        return onSuccess(result.Value!);
    }

    /// <summary>
    /// Shortcut for creating a 201 Created response with an IdResponse body.
    /// </summary>
    /// <param name="result">The result containing the ID.</param>
    /// <param name="locationHelper">A function that takes the ID and returns the URL string (or null).</param>
    public static IActionResult ToCreatedIdResponse<T>(
        this Result<T> result,
        Func<T, string>? locationHelper = null)
    {
        if (result.IsFailure)
            return result.ToErrorActionResult();

        var response = new IdResponse<T>(result.Value);
        var location = locationHelper?.Invoke(result.Value!) ?? string.Empty;

        return new CreatedResult(location, response);
    }
}