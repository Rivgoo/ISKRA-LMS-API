namespace Iskra.Modules.Validation.Abstractions.Options.Users;

public class UserValidationOptions
{
    public int MinFirstNameLength { get; set; } = 2;
    public int MaxFirstNameLength { get; set; } = 100;

    public int MinLastNameLength { get; set; } = 2;
    public int MaxLastNameLength { get; set; } = 100;

    public int MinMiddleNameLength { get; set; } = 0;
    public int MaxMiddleNameLength { get; set; } = 100;
}