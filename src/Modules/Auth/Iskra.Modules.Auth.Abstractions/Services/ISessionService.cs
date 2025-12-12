using Iskra.Application.Results;
using Iskra.Modules.Auth.Abstractions.Models;

namespace Iskra.Modules.Auth.Abstractions.Services;

public interface ISessionService
{
    Task<Result> StartSessionAsync(Guid userId, DeviceInfo device, CancellationToken ct = default);
    Task<Result> RefreshSessionAsync(string refreshToken, DeviceInfo device, CancellationToken ct = default);
    Task<Result> TerminateCurrentSessionAsync(CancellationToken ct = default);
    Task<Result> TerminateAllOtherSessionsAsync(CancellationToken ct = default);
}