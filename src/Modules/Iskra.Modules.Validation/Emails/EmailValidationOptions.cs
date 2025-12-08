namespace Iskra.Modules.Validation.Emails;

public class EmailValidationOptions
{
    public string RegexPattern { get; set; } = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    public int MaxLength { get; set; } = 100;
    public List<string> AllowedDomains { get; set; } = ["gmail.com"];
}