using Iskra.Application.Errors;

namespace Iskra.Modules.Auth.Password.Abstractions.Errors;

public static class CredentialsErrors
{
    /// <summary>
    /// Generic error returned when login fails due to incorrect email or password.
    /// Does not reveal which part was incorrect to prevent user enumeration.
    /// </summary>
    public static Error InvalidCredentials
        => Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password.");
}