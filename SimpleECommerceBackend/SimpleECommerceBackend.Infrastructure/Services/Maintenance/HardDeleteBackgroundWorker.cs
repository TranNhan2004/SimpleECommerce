using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;


namespace SimpleECommerceBackend.Infrastructure.Services.Maintenance;

public sealed class HardDeleteBackgroundWorker : BackgroundService
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly TimeProvider _timeProvider;

    public HardDeleteBackgroundWorker(
        IServiceScopeFactory serviceScopeFactory,
        TimeProvider timeProvider,
        ILogger logger
    )
    {
        _serviceScopeFactory = serviceScopeFactory;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = _timeProvider.GetUtcNow();
            var nextRun = HardDeleteSchedule.GetNextRun(now, _timeProvider.LocalTimeZone);
            var delay = nextRun - now;

            if (delay > TimeSpan.Zero)
            {
                _logger.Information(
                    "Hard delete cleanup worker scheduled next run at {NextRun}.",
                    nextRun
                );

                await Task.Delay(delay, stoppingToken);
            }

            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var cleanupService = scope.ServiceProvider.GetRequiredService<HardDeleteCleanupService>();

                _logger.Information("Hard delete cleanup worker started at {StartedAt}.", _timeProvider.GetUtcNow());
                await cleanupService.RunAsync(stoppingToken);
                _logger.Information("Hard delete cleanup worker finished at {FinishedAt}.", _timeProvider.GetUtcNow());
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Hard delete cleanup worker failed.");
            }
        }
    }
}
