namespace Iskra.Modules.Users.Abstractions.Models.Requests;

public record RegisterUserRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Role,
    string? MiddleName = null,
    bool IsEmailConfirmed = false
);