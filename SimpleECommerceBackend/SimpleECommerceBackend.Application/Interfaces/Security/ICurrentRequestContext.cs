namespace SimpleECommerceBackend.Application.Interfaces.Security;

/// <summary>
/// Represents request metadata used for auditing and logging.
/// Safe to use for anonymous requests and background execution.
/// </summary>
public interface ICurrentRequestContext
{
    Guid ActorId { get; }
    string TraceId { get; }
    string IpAddress { get; }
}
