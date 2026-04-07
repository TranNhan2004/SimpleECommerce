using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Business;

public interface IProductImageRepository : IGenericRepository<ProductImage>
{
    Task<IReadOnlyList<ProductImage>> FindByProductIdAsync(Guid productId);
    Task<ProductImage?> FindByProductIdAndDisplayOrderAsync(Guid productId, int displayOrder);
}