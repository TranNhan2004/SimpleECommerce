using SimpleECommerceBackend.Domain.Entities;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<Product?> FindByIdAsync(Guid id);
    Task<IReadOnlyList<Product>> FindAllAsync();
    Product Add(Product product);
    Product Update(Product product);
    Product Delete(Product product);
}
