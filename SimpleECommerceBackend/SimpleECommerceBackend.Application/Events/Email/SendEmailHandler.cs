using MediatR;
using Microsoft.Extensions.Logging;
using SimpleECommerceBackend.Application.Interfaces.Services.Email;

namespace SimpleECommerceBackend.Application.Events.Email;

public sealed record SendEmailEvent(
    string Email,
    string EmailBody
) : INotification;

[AutoConstructor]
public sealed partial class SendEmailEventHandler : INotificationHandler<SendEmailEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<SendEmailEventHandler> _logger;

    public async Task Handle(
        SendEmailEvent notification,
        CancellationToken cancellationToken
    )
    {
        await _emailService.SendAsync(
            notification.Email,
            "Welcome to SimpleECommerce",
            notification.EmailBody
        );

        _logger.LogInformation(
            "[Events::SendEmailHandler]: Welcome email queued for {Email}",
            notification.Email
        );
    }
}