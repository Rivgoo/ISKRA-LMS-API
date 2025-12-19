namespace Iskra.Modules.Users.Abstractions.Models.Responses;

/// <summary>
/// Represents basic profile data visible to other members.
/// </summary>
public record UserPublicResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? MiddleName
);