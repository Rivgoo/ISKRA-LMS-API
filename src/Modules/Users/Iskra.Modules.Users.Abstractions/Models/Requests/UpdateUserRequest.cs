namespace Iskra.Modules.Users.Abstractions.Models.Requests;

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string? MiddleName
);