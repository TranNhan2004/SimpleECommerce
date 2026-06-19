namespace SimpleECommerceBackend.Application.Interfaces.Events;

public interface IEventHandler<in TEvent>
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}