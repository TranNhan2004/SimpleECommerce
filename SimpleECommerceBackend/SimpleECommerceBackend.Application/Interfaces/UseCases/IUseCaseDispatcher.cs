namespace SimpleECommerceBackend.Application.Interfaces.UseCases;

public interface IUseCaseDispatcher
{
    Task<TResult> SendAsync<TRequest, TResult>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class
        where TResult : class;

    Task SendAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class;
}