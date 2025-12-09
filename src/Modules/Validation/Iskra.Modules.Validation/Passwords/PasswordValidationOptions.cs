namespace Iskra.Modules.Validation.Passwords;

public class PasswordValidationOptions
{
    public int MinLength { get; set; } = 8;
    public int MaxLength { get; set; } = 128;
    public bool RequireDigit { get; set; } = true;
    public bool RequireLowercase { get; set; } = true;
    public bool RequireUppercase { get; set; } = true;
    public bool RequireNonAlphanumeric { get; set; } = false;
    public string? CustomRegexPattern { get; set; }
}