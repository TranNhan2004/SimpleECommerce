using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SimpleECommerceBackend.Application.Interfaces.Services.Email;

namespace SimpleECommerceBackend.Infrastructure.Services.Email;

[AutoConstructor]
public partial class SmtpEmailSender : IEmailSender
{
    private readonly IOptions<SmtpOptions> _smtpOptions;

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken stoppingToken)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_smtpOptions.Value.From));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();

        await client.ConnectAsync(
            _smtpOptions.Value.Host,
            _smtpOptions.Value.Port,
            SecureSocketOptions.StartTls,
            stoppingToken
        );

        await client.AuthenticateAsync(
            _smtpOptions.Value.Username,
            _smtpOptions.Value.Password,
            stoppingToken
        );

        await client.SendAsync(message, stoppingToken);
        await client.DisconnectAsync(true, stoppingToken);
    }
}