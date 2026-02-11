using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

public interface IProductRepository
{
    Task<Product?> FindByIdAsync(Guid id);
    Task<IEnumerable<Product>> FindAllAsync(); 
    // Simplified for now, will add pagination/filtering in implementation or specific method
    Product Add(Product product);
    Product Update(Product product);
    Product Delete(Product product);
}
