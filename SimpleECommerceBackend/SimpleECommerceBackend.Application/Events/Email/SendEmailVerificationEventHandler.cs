using MediatR;
using Microsoft.Extensions.Logging;
using SimpleECommerceBackend.Application.Interfaces.Services.Email;

namespace SimpleECommerceBackend.Application.Events.Email;

public sealed record SendEmailVerificationEvent(
    string Email,
    string Token
) : INotification;

[AutoConstructor]
public sealed partial class SendEmailVerificationEventHandler : INotificationHandler<SendEmailVerificationEvent>
{
    private readonly IEmailService _emailService;
    private readonly IEmailProvider _emailProvider;
    private readonly IEmailVerificationLinkBuilder _emailVerificationLinkBuilder;
    private readonly ILogger<SendEmailVerificationEventHandler> _logger;

    public async Task Handle(
        SendEmailVerificationEvent notification,
        CancellationToken cancellationToken
    )
    {
        var verificationUrl = _emailVerificationLinkBuilder.BuildConfirmationUrl(notification.Token);
        var emailBody = _emailProvider.BuildAccountVerificationEmail(verificationUrl);

        await _emailService.SendAsync(
            notification.Email,
            "Verify your SimpleECommerce account",
            emailBody
        );

        _logger.LogInformation(
            "[Events::SendEmailVerificationEventHandler]: Verification email queued for {Email}",
            notification.Email
        );
    }
}