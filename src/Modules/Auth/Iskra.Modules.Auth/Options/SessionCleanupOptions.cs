namespace Iskra.Modules.Auth.Options;

public class SessionCleanupOptions
{
    /// <summary>
    /// Enables or disables the background cleanup job.
    /// Default: true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// How often the job runs (in hours).
    /// Default: 24 hours.
    /// </summary>
    public int RunIntervalHours { get; set; } = 24;

    /// <summary>
    /// How old a revoked/expired session must be before it is hard-deleted from DB (in days).
    /// Default: 30 days (keep history for a month).
    /// </summary>
    public int RetentionDays { get; set; } = 30;
}