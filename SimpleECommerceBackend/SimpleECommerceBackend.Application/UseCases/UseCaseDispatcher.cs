using Microsoft.Extensions.DependencyInjection;
using SimpleECommerceBackend.Application.Interfaces.UseCases;

namespace SimpleECommerceBackend.Application.UseCases;

public partial class UseCaseDispatcher : IUseCaseDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public UseCaseDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResult> SendAsync<TRequest, TResult>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class
        where TResult : class
    {
        var handler = _serviceProvider.GetRequiredService<IUseCaseHandler<TRequest, TResult>>();
        return await handler.HandleAsync(request, cancellationToken);
    }

    public async Task SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class
    {
        var handler = _serviceProvider.GetRequiredService<IUseCaseHandler<TRequest>>();
        await handler.HandleAsync(request, cancellationToken);
    }
}