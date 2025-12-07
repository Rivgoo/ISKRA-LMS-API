namespace Iskra.Modules.Validation.Users;

/// <summary>
/// Holds the validation rules for a User entity, loaded from configuration.
/// </summary>
public class UserValidationOptions
{
    public int MinFirstNameLength { get; set; } = 2;
    public int MaxFirstNameLength { get; set; } = 100;
    public int MinPasswordLength { get; set; } = 8;
}