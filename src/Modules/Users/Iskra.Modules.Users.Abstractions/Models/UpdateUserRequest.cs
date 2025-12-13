namespace Iskra.Modules.Users.Abstractions.Models;

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string? MiddleName
);