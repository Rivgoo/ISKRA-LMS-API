using Iskra.Api.Abstractions.Models;
using Microsoft.AspNetCore.Http;

namespace Iskra.Api.Abstractions.Extensions;

public static class HttpContextExtensions
{
    public static DeviceInfo GetDeviceInfo(this HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString();

        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
            ip = forwarded.FirstOrDefault();

        var ua = context.Request.Headers.UserAgent.ToString();
        if (string.IsNullOrEmpty(ua)) ua = "Unknown";

        return new DeviceInfo(ip, ua);
    }
}