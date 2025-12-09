using Iskra.Bootstrapper.Options;
using Microsoft.Extensions.Logging;

namespace Iskra.Bootstrapper.Security;

internal static class SecurityChecks
{
    public static void PerformChecks(SecurityOptions options, ILogger logger)
    {
        CheckAllowedHosts(options, logger);
    }

    private static void CheckAllowedHosts(SecurityOptions options, ILogger logger)
    {
        var hostConfig = options.HostFiltering;

        if (string.IsNullOrEmpty(hostConfig.AllowedHosts) || hostConfig.AllowedHosts == "*")
            logger.LogWarning("SECURITY ALERT: 'Security:HostFiltering:AllowedHosts' is set to wildcard (*). Vulnerable to Host Header attacks.");
    }
}