using SimpleECommerceBackend.Application.Models.Common.Filter;
using SimpleECommerceBackend.Application.Models.Products;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Business;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<FilterResult<ProductItemForCustomer>> FindAllWithFilterForCustomerAsync(FilterQuery<Product> query);
}