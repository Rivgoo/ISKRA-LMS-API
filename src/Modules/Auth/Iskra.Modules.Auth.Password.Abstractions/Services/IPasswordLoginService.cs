using Iskra.Api.Abstractions.Models;
using Iskra.Application.Results;
using Iskra.Modules.Auth.Password.Abstractions.Requests;

namespace Iskra.Modules.Auth.Password.Abstractions.Services;

public interface IPasswordLoginService
{
    Task<Result> LoginAsync(LoginRequest request, DeviceInfo device, CancellationToken ct);
}