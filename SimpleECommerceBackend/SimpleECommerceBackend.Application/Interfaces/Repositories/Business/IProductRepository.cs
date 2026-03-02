using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Business;

public interface IProductRepository
{
    Task<Product?> FindByIdAsync(Guid id);
    Task<IReadOnlyList<Product>> FindAllAsync(); 
    Product Add(Product product);
    Product Update(Product product);
    Product Delete(Product product);
}
