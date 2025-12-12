namespace Iskra.Modules.Auth.Password.Abstractions.Requests;

public record LoginRequest(string Email, string Password);