namespace Iskra.Bootstrapper.Options.RateLimiting;

public class RateLimitPolicyConfig
{
    /// <summary>
    /// The name used in [EnableRateLimiting("Name")] attributes.
    /// Leave null for Global policy.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Maximum number of requests allowed in the window.
    /// </summary>
    public int PermitLimit { get; set; } = 100;

    /// <summary>
    /// The time window in seconds.
    /// </summary>
    public int WindowSeconds { get; set; } = 60;

    /// <summary>
    /// Maximum number of requests to queue if the limit is hit. 
    /// Set to 0 to reject immediately.
    /// </summary>
    public int QueueLimit { get; set; } = 0;
}