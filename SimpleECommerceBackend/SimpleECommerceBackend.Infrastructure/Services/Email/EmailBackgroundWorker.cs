using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimpleECommerceBackend.Application.Interfaces.Services.Email;

namespace SimpleECommerceBackend.Infrastructure.Services.Email;

[AutoConstructor]
public partial class EmailBackgroundWorker : BackgroundService
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<EmailBackgroundWorker> _logger;
    private readonly BackgroundEmailQueue _queue;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                var (to, subject, body) = await _queue.DequeueAsync(stoppingToken);
                await _emailSender.SendEmailAsync(to, subject, body, stoppingToken);
                _logger.LogInformation("Email send succeeded to {To}", to);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email send failed.");
            }
    }
}