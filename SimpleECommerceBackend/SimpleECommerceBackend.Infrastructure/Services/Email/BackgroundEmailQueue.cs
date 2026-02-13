using System.Threading.Channels;

namespace SimpleECommerceBackend.Infrastructure.Services.Email;

public class BackgroundEmailQueue
{
    private readonly Channel<(string To, string Subject, string Body)> _queue;

    public BackgroundEmailQueue()
    {
        _queue = Channel.CreateBounded<(string To, string Subject, string Body)>(
            new BoundedChannelOptions(5000)
            {
                FullMode = BoundedChannelFullMode.Wait
            });
    }

    public async ValueTask EnqueueAsync(string to, string subject, string body)
    {
        if (string.IsNullOrEmpty(to))
            throw new ArgumentNullException(nameof(to));

        await _queue.Writer.WriteAsync((to, subject, body));
    }

    public async ValueTask<(string To, string Subject, string Body)> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}