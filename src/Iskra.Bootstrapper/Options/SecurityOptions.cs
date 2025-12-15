using Iskra.Bootstrapper.Options.RateLimiting;

namespace Iskra.Bootstrapper.Options;

public class SecurityOptions
{
    public const string SectionName = "Security";
    public HostFilteringConfig HostFiltering { get; set; } = new();
    public CorsOptions Cors { get; set; } = new();
    public RateLimitingOptions RateLimiting { get; set; } = new();
}