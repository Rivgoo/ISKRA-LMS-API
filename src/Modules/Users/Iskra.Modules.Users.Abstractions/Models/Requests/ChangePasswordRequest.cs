namespace Iskra.Modules.Users.Abstractions.Models.Requests;

public record ChangePasswordRequest(string OldPassword, string NewPassword);