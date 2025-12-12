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
    private static IActionResult ToErrorActionResult(this Result result)
    {
        var error = result.Error ?? Error.Failure("Unknown.Error", "An unknown error occurred.");

        var problemDetails = new ProblemDetails
        {
            Status = (int)error.ToHttpStatusCode(),
            Title = error.Code,
            Detail = error.Description,
            Type = error.ErrorType.ToString()
        };

        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status,
            ContentTypes = { "application/problem+json" }
        };
    }
}