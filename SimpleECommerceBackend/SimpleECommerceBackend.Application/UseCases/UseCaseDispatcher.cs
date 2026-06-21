using Microsoft.Extensions.DependencyInjection;
using SimpleECommerceBackend.Application.Interfaces.UseCases;

namespace SimpleECommerceBackend.Application.UseCases;

public partial class UseCaseDispatcher : IUseCaseDispatcher
{
    private readonly Serilog.ILogger _logger;
    private readonly IServiceProvider _serviceProvider;

    public UseCaseDispatcher(Serilog.ILogger logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task<TResult> SendAsync<TRequest, TResult>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class
        where TResult : class
    {
        var handler = _serviceProvider.GetRequiredService<IUseCaseHandler<TRequest, TResult>>()
            ?? throw new InvalidOperationException($"No handler found for request of type <{typeof(TRequest).FullName}, {typeof(TResult).FullName}>");

        _logger.Information("UseCaseDispatcher: found handler {FullName} for request of type <{RequestType}, {ResultType}>",
            handler.GetType().FullName,
            typeof(TRequest).FullName,
            typeof(TResult).FullName
        );
        return await handler.HandleAsync(request, cancellationToken);
    }

    public async Task SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class
    {
        var handler = _serviceProvider.GetRequiredService<IUseCaseHandler<TRequest>>()
            ?? throw new InvalidOperationException($"No handler found for request of type <{typeof(TRequest).FullName}>");

        _logger.Information("UseCaseDispatcher: found handler {FullName} for request of type <{RequestType}>",
            handler.GetType().FullName,
            typeof(TRequest).FullName
        );
        await handler.HandleAsync(request, cancellationToken);
    }
}
