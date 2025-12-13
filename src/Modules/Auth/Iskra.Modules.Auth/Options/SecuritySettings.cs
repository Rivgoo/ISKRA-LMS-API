namespace Iskra.Modules.Auth.Options;

internal class SecuritySettings
{
    /// <summary>
    /// If true, users cannot start a session until IsEmailConfirmed is true.
    /// Default: true.
    /// </summary>
    public bool RequireEmailConfirmation { get; set; } = true;
}