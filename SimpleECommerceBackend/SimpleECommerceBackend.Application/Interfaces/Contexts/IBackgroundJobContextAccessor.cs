namespace SimpleECommerceBackend.Application.Interfaces.Contexts;

public interface IBackgroundJobContextAccessor
{
    string? JobName { get; }
    string? TraceId { get; }
    IDisposable BeginScope(string jobName, string? traceId = null);
}
