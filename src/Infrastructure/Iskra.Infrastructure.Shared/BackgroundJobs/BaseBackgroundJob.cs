using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Iskra.Infrastructure.Shared.BackgroundJobs;

/// <summary>
/// A base class for creating periodic background tasks based on a timer.
/// Handles execution loops, error catching, and logging automatically.
/// </summary>
public abstract class BaseBackgroundJob : BackgroundService
{
    private readonly ILogger _logger;

    protected BaseBackgroundJob(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// The interval between executions.
    /// </summary>
    protected abstract TimeSpan Period { get; }

    /// <summary>
    /// If true, the job is enabled. If false, ExecuteAsync terminates immediately.
    /// </summary>
    protected virtual bool IsEnabled => true;

    /// <summary>
    /// The actual work logic to be executed periodically.
    /// </summary>
    protected abstract Task ExecuteWorkAsync(CancellationToken stoppingToken);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!IsEnabled)
        {
            _logger.LogInformation("Job {JobName} is disabled by configuration.", GetType().Name);
            return;
        }

        _logger.LogInformation("Job {JobName} started. Period: {Period}", GetType().Name, Period);

        using var timer = new PeriodicTimer(Period);

        while (await timer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExecuteWorkAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing job {JobName}.", GetType().Name);
            }
        }

        _logger.LogInformation("Job {JobName} stopped.", GetType().Name);
    }
}