using SimpleECommerceBackend.Application.Models.Products;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Services.Business;

public interface IProductService : ICacheConsumingService
{
    Product CreateProduct(Product product);
    Task<GetAllProductsResultForCustomer> GetAllProductsForCustomerAsync(GetAllProductsQueryForCustomer query);
    Task<Product> GetProductByIdAsync(Guid id);
    Task<Product> GetProductByIdForUpdateAsync(Guid id);
}