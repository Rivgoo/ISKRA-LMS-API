namespace Iskra.Application.Errors;

public static class PasswordErrors
{
    public static Error TooShort(int minLength)
        => Error.BadRequest("Password.TooShort", "Password must be at least {0} characters long.", minLength);

    public static Error TooLong(int maxLength)
        => Error.BadRequest("Password.TooLong", "Password must not exceed {0} characters.", maxLength);

    public static Error RequiresDigit
        => Error.BadRequest("Password.RequiresDigit", "Password must contain at least one digit ('0'-'9').");

    public static Error RequiresLower
        => Error.BadRequest("Password.RequiresLower", "Password must contain at least one lowercase letter ('a'-'z').");

    public static Error RequiresUpper
        => Error.BadRequest("Password.RequiresUpper", "Password must contain at least one uppercase letter ('A'-'Z').");

    public static Error RequiresNonAlphanumeric
        => Error.BadRequest("Password.RequiresNonAlphanumeric", "Password must contain at least one non-alphanumeric character.");

    public static Error InvalidFormat
        => Error.BadRequest("Password.InvalidFormat", "Password does not meet the complexity requirements.");

    public static Error Mismatch
        => Error.BadRequest("Password.Mismatch", "The provided password does not match the stored hash.");
}