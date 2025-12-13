namespace Iskra.Modules.Users.Abstractions.Models;

public record ChangePasswordRequest(string OldPassword, string NewPassword);