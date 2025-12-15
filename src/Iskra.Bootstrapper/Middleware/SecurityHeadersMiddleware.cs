using Iskra.Bootstrapper.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Iskra.Bootstrapper.Middleware;

public class SecurityHeadersMiddleware(RequestDelegate next, IOptions<SecurityOptions> options)
{
    private readonly SecurityHeadersOptions _headers = options.Value.Headers;

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Remove Leaky Headers
        context.Response.Headers.Remove("X-Powered-By");
        context.Response.Headers.Remove("Server");

        // 2. Add Security Headers
        var headers = context.Response.Headers;

        // Prevent MIME Sniffing
        if (!string.IsNullOrEmpty(_headers.XContentTypeOptions))
            headers.Append("X-Content-Type-Options", _headers.XContentTypeOptions);

        // Content Security Policy (CSP)
        if (!string.IsNullOrEmpty(_headers.ContentSecurityPolicy))
            headers.Append("Content-Security-Policy", _headers.ContentSecurityPolicy);

        await next(context);
    }
}