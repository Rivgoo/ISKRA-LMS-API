namespace Iskra.Bootstrapper.Options;

public class HostFilteringConfig
{
    /// <summary>
    /// Semicolon-separated list of allowed host names. 
    /// Use "*" to allow all.
    /// </summary>
    public string AllowedHosts { get; set; } = "*";

    /// <summary>
    /// If true, allows requests without a Host header.
    /// Recommended: false for production.
    /// </summary>
    public bool AllowEmptyHosts { get; set; } = false;

    /// <summary>
    /// If true, returns a detailed 400 Bad Request message.
    /// Recommended: false for production to avoid leaking info.
    /// </summary>
    public bool IncludeFailureMessage { get; set; } = false;
}