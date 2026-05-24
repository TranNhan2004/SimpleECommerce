namespace SimpleECommerceBackend.Application.Interfaces.Contexts;

public interface ICurrentRequestContext
{
    string UserId { get; }
    string TraceId { get; }
    string IpAddress { get; }
}
