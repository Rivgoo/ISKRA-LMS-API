using Iskra.Api.Abstractions.Models;
using Iskra.Application.Results;

namespace Iskra.Modules.Auth.Abstractions.Services;

public interface ISessionService
{
    Task<Result> StartSessionAsync(Guid userId, DeviceInfo device, CancellationToken ct = default);
    Task<Result> RefreshSessionAsync(string refreshToken, DeviceInfo device, CancellationToken ct = default);
    Task<Result> TerminateCurrentSessionAsync(CancellationToken ct = default);
    Task<Result> TerminateAllOtherSessionsAsync(CancellationToken ct = default);
}