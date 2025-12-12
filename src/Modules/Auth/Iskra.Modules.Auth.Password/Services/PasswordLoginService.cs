using Iskra.Api.Abstractions.Models;
using Iskra.Application.Abstractions.Security;
using Iskra.Application.Results;
using Iskra.Modules.Auth.Abstractions.Services;
using Iskra.Modules.Auth.Password.Abstractions.Errors;
using Iskra.Modules.Auth.Password.Abstractions.Requests;
using Iskra.Modules.Auth.Password.Abstractions.Services;
using Iskra.Modules.Users.Abstractions.Repositories;

namespace Iskra.Modules.Auth.Password.Services;

internal sealed class PasswordLoginService(
	IUserRepository userRepository,
	IPasswordHasher passwordHasher,
	ISessionService sessionService) : IPasswordLoginService
{
	public async Task<Result> LoginAsync(LoginRequest request, DeviceInfo device, CancellationToken ct)
	{
		// 1. Check Empty Password explicitly (Defensive coding)
		if (string.IsNullOrWhiteSpace(request.Password))
			return CredentialsErrors.InvalidCredentials;

		// 2. Find User
		var user = await userRepository.GetByEmailAsync(request.Email, ct);

		// 3. Verify Credentials
		if (user is null)
			return CredentialsErrors.InvalidCredentials;

		if (string.IsNullOrEmpty(user.PasswordHash))
			return CredentialsErrors.InvalidCredentials;

		if (!passwordHasher.Verify(request.Password, user.PasswordHash))
			return CredentialsErrors.InvalidCredentials;

		// 4. Start Session via Core Auth Module
		return await sessionService.StartSessionAsync(user.Id, device, ct);
	}
}