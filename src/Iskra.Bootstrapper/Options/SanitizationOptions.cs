namespace Iskra.Bootstrapper.Options;

public class SanitizationOptions
{
    /// <summary>
    /// Master switch for input sanitization.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// List of property names that should NEVER be sanitized (case-insensitive).
    /// e.g. "Password", "PasswordHash", "Secret".
    /// </summary>
    public List<string> ExcludedProperties { get; set; } =
    [
        "Password",
        "OldPassword",
        "NewPassword"
    ];
}