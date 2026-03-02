namespace SimpleECommerceBackend.Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}