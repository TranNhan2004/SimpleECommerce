namespace SimpleECommerceBackend.Application.Interfaces.UseCases;

public interface IUseCaseHandler<in TRequest, TResult>
    where TRequest : class
    where TResult : class
{
    Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken);
}

public interface IUseCaseHandler<in TRequest>
    where TRequest : class
{
    Task HandleAsync(TRequest request, CancellationToken cancellationToken);
}