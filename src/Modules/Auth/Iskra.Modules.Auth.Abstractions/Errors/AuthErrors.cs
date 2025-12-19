using Iskra.Application.Errors;

namespace Iskra.Modules.Auth.Abstractions.Errors;

public static class AuthErrors
{
    public static Error Forbidden
        => Error.AccessForbidden("Auth.Forbidden", "Insufficient permissions to access this information.");

    public static Error InvalidSession
        => Error.Unauthorized("Auth.InvalidSession", "The session is invalid or expired.");

    public static Error SessionRevoked
        => Error.Unauthorized("Auth.SessionRevoked", "This session has been revoked.");

    public static Error UserInactive
        => Error.Unauthorized("Auth.UserInactive", "User account is deactivated.");

    public static Error MissingToken
        => Error.Unauthorized("Auth.MissingToken", "No authentication token provided.");

    public static Error EmailNotConfirmed
        => Error.Unauthorized("Auth.EmailNotConfirmed", "Please confirm your email address before logging in.");
}