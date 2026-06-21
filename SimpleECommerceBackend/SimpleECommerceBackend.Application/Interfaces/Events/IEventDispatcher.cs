namespace SimpleECommerceBackend.Application.Interfaces.Events;

public interface IEventDispatcher
{
    Task SendAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default);
}