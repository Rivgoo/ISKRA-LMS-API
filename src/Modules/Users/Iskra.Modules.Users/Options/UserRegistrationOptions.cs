namespace Iskra.Modules.Users.Options;

public class UserRegistrationOptions
{
    /// <summary>
    /// If true, allows creating a user without a password hash (e.g. for SSO-only users).
    /// If false, creation fails if PasswordHash is empty.
    /// Default: false.
    /// </summary>
    public bool AllowPasswordlessRegistration { get; set; } = false;
}