using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using SimpleECommerceBackend.Application.Interfaces.Services.Email;

namespace SimpleECommerceBackend.Infrastructure.Services.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _smtpOptions;

    public SmtpEmailSender(IConfiguration configuration)
    {
        _smtpOptions = configuration.GetSection("SmtpOptions").Get<SmtpOptions>()!;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken stoppingToken)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_smtpOptions.From));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();

        await client.ConnectAsync(
            _smtpOptions.Host,
            _smtpOptions.Port,
            SecureSocketOptions.StartTls,
            stoppingToken
        );

        await client.AuthenticateAsync(
            _smtpOptions.Username,
            _smtpOptions.Password,
            stoppingToken
        );

        await client.SendAsync(message, stoppingToken);
        await client.DisconnectAsync(true, stoppingToken);
    }
}