namespace Iskra.Bootstrapper.Options.RateLimiting;

public class RateLimitingOptions
{
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// The HTTP Status code to return when limit is exceeded. Default 429.
    /// </summary>
    public int RejectionStatusCode { get; set; } = 429;

    /// <summary>
    /// Configuration for the global limiter applied to all requests.
    /// </summary>
    public RateLimitPolicyConfig? Global { get; set; }

    /// <summary>
    /// List of specific named policies (e.g., "AuthPolicy") to be applied to specific controllers.
    /// </summary>
    public List<RateLimitPolicyConfig> Policies { get; set; } = [];
}