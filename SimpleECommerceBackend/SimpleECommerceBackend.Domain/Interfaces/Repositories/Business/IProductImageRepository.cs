using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Domain.Interfaces.Repositories.Business;

public interface IProductImageRepository
{
    Task<IEnumerable<ProductImage>> FindByProductIdAsync(Guid productId);
    Task<ProductImage?> FindAsync(Guid productId, int displayOrder);
    ProductImage Add(ProductImage image);
    void Delete(ProductImage image);
    void DeleteAllByProductId(Guid productId);
}
