using SimpleECommerceBackend.Application.Interfaces.Services.Email;

namespace SimpleECommerceBackend.Infrastructure.Services.Email;

[AutoConstructor]
public partial class SmtpEmailService : IEmailService
{
    private readonly BackgroundEmailQueue _queue;

    public ValueTask SendAsync(string to, string subject, string body)
    {
        return _queue.EnqueueAsync(to, subject, body);
    }
}