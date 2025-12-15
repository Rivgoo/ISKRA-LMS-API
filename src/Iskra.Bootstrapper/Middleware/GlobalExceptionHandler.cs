using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Iskra.Bootstrapper.Middleware;

/// <summary>
/// A centralized exception handler that catches unhandled exceptions from the pipeline.
/// Maps Domain Invariant exceptions (ArgumentException, InvalidOperationException) to appropriate HTTP responses.
/// </summary>
internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.Request.Path
        };

        if (exception is DbUpdateConcurrencyException)
        {
            logger.LogWarning("Concurrency violation detected: {Message}", exception.Message);

            problemDetails.Title = "Resource Conflict";
            problemDetails.Status = StatusCodes.Status409Conflict;
            problemDetails.Detail = "The resource you are trying to update has been modified by another user. Please reload and try again.";
            problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8";
        }
        else
        {
            logger.LogError(exception, "Unhandled Exception: {Message}", exception.Message);

            problemDetails.Title = "Internal Server Error";
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Detail = "An unexpected error occurred. Please contact support.";
            problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}