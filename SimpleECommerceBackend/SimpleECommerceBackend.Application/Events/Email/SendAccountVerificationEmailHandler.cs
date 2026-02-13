using MediatR;
using Microsoft.Extensions.Logging;
using SimpleECommerceBackend.Application.Interfaces.Services.Email;

namespace SimpleECommerceBackend.Application.Events.Email;

public sealed record UserRegisteredEvent(
    string Email,
    string VerficationUrl
) : INotification;

[AutoConstructor]
public sealed partial class UserRegisteredEventHandler
    : INotificationHandler<UserRegisteredEvent>
{
    private readonly IEmailProvider _emailProvider;
    private readonly IEmailService _emailService;
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public async Task Handle(
        UserRegisteredEvent notification,
        CancellationToken cancellationToken)
    {
        var emailBody = _emailProvider.BuildAccountVerificationEmail(notification.VerficationUrl);

        await _emailService.SendAsync(
            notification.Email,
            "Welcome to SimpleECommerce",
            emailBody
        );

        _logger.LogInformation(
            "Welcome email queued for {Email}",
            notification.Email
        );
    }
}