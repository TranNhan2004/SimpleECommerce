using SimpleECommerceBackend.Application.Interfaces.Services.Email;

namespace SimpleECommerceBackend.Infrastructure.Services.Email;

public class SmtpEmailService : IEmailService
{
    private readonly BackgroundEmailQueue _queue;

    public SmtpEmailService(BackgroundEmailQueue queue)
    {
        _queue = queue;
    }

    public ValueTask SendAsync(string to, string subject, string body)
    {
        return _queue.EnqueueAsync(to, subject, body);
    }
}