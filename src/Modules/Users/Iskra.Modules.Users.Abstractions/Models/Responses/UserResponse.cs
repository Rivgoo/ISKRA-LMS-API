namespace Iskra.Modules.Users.Abstractions.Models.Responses;

/// <summary>
/// Represents a comprehensive account data set including security and system fields.
/// </summary>
public record UserResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? MiddleName,
    bool IsActive,
    bool IsEmailConfirmed,
    List<string> Roles,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);