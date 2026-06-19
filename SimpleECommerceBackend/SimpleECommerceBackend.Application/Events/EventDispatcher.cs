using SimpleECommerceBackend.Application.Interfaces.Events;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleECommerceBackend.Application.Events;

public class EventDispatcher : IEventDispatcher
{

    private readonly Serilog.ILogger _logger;
    private readonly IServiceProvider _serviceProvider;

    public EventDispatcher(Serilog.ILogger logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task SendAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
    {
        var handler = _serviceProvider.GetRequiredService<IEventHandler<TEvent>>()
            ?? throw new InvalidOperationException($"No handler found for request of type <{typeof(TEvent).FullName}>");

        _logger.Information("EventDispatcher: found handler {FullName} for request of type <{EventType}>",
            handler.GetType().FullName,
            typeof(TEvent).FullName
        );

        await handler.HandleAsync(@event, cancellationToken);
    }
}