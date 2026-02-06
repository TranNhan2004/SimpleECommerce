using SimpleECommerceBackend.Domain.Interfaces.Time;

namespace SimpleECommerceBackend.Infrastructure.Time;

public class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}