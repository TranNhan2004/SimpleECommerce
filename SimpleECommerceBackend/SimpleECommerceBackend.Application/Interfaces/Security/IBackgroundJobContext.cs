namespace SimpleECommerceBackend.Application.Interfaces.Security;

/// <summary>
/// Stores ambient metadata for background job execution.
/// Create a scope at the start of a job so downstream services can resolve job identity and trace id.
/// </summary>
public interface IBackgroundJobContext
{
    string? JobName { get; }
    string? TraceId { get; }
    IDisposable BeginScope(string jobName, string? traceId = null);
}
