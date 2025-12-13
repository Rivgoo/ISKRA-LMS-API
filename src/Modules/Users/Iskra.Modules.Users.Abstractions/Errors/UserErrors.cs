namespace Iskra.Application.Errors.DomainErrors;

public static class UserErrors
{
    public static Error NotFoundByEmail(string email)
        => Error.NotFound("User.NotFoundByEmail", "User with email '{0}' was not found.", email);

    public static Error UpdatePasswordFailed
        => Error.Failure("User.UpdatePasswordFailed", "Failed to update the password hash in the database.");

    public static Error SetActiveStatusFailed
        => Error.Failure("User.SetActiveStatusFailed", "Failed to update the user active status.");

    public static Error EmailAlreadyExists(string email)
        => Error.Conflict("User.EmailAlreadyExists", "The email '{0}' is already in use.", email);

    public static Error PasswordRequired
        => Error.BadRequest("User.PasswordRequired", "Password is required for registration by system policy.");

    public static Error RoleRequired
        => Error.BadRequest("User.RoleRequired", "A user role must be specified during registration.");

    public static Error InvalidOldPassword
        => Error.Unauthorized("User.InvalidOldPassword", "The old password is incorrect.");

    public static Error CannotDeleteSelf
        => Error.Conflict("User.CannotDeleteSelf", "You cannot delete your own account via this API.");
}