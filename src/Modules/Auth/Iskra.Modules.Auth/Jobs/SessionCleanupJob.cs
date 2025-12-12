using Iskra.Infrastructure.Shared.BackgroundJobs;
using Iskra.Modules.Auth.Abstractions.Repositories;
using Iskra.Modules.Auth.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Iskra.Modules.Auth.Jobs;

internal sealed class SessionCleanupJob : BaseBackgroundJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly SessionCleanupOptions _options;
    private readonly ILogger<SessionCleanupJob> _logger;

    public SessionCleanupJob(
        ILogger<SessionCleanupJob> logger,
        IServiceProvider serviceProvider,
        IOptions<AuthOptions> authOptions)
        : base(logger)
    {
        _serviceProvider = serviceProvider;
        _options = authOptions.Value.SessionCleanup;
        _logger = logger;
    }

    protected override TimeSpan Period => TimeSpan.FromHours(_options.RunIntervalHours);

    protected override bool IsEnabled => _options.Enabled;

    protected override async Task ExecuteWorkAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

        var cutoffDate = DateTimeOffset.UtcNow.AddDays(-_options.RetentionDays);

        var deletedCount = await repository.DeleteExpiredSessionsAsync(cutoffDate, stoppingToken);

        if (deletedCount > 0)
            _logger.LogInformation("Deleted {DeletedCount} expired sessions older than {CutoffDate} UTC.", deletedCount, cutoffDate);
    }
}