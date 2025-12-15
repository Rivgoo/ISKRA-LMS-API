using Iskra.Bootstrapper.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace Iskra.Bootstrapper.Extensions;

internal static class RateLimitingExtensions
{
    public static void AddIskraRateLimiting(this IServiceCollection services, SecurityOptions options)
    {
        var settings = options.RateLimiting;

        if (!settings.Enabled) return;

        services.AddRateLimiter(limiterOptions =>
        {
            limiterOptions.RejectionStatusCode = settings.RejectionStatusCode;

            limiterOptions.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = settings.RejectionStatusCode;

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();

                // Create RFC 7807 ProblemDetails
                var problemDetails = new ProblemDetails
                {
                    Status = settings.RejectionStatusCode,
                    Title = "Too Many Requests",
                    Detail = "You have exceeded the allowed request limit. Please wait before retrying.",
                    Type = "https://tools.ietf.org/html/rfc6585#section-4",
                    Instance = context.HttpContext.Request.Path
                };

                await context.HttpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: token);
            };

            // 1. Configure Global Limiter (Applied to ALL endpoints)
            if (settings.Global != null)
            {
                limiterOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    // Partition by IP Address
                    var remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: remoteIp,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = settings.Global.PermitLimit,
                            Window = TimeSpan.FromSeconds(settings.Global.WindowSeconds),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = settings.Global.QueueLimit
                        });
                });
            }

            // 2. Configure Named Policies (Applied via Attribute)
            foreach (var policy in settings.Policies)
            {
                limiterOptions.AddPolicy(policy.Name, context =>
                {
                    var remoteIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: remoteIp,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = policy.PermitLimit,
                            Window = TimeSpan.FromSeconds(policy.WindowSeconds),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = policy.QueueLimit
                        });
                });
            }
        });
    }
}