using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Business;

public interface IProductVariantImageRepository : IGenericRepository<ProductVariantImage>
{
    Task<IReadOnlyList<ProductVariantImage>> FindByProductVariantIdAsync(Guid productVariantId);
    Task<ProductVariantImage?> FindByProductVariantIdAndDisplayOrderAsync(Guid productVariantId, int displayOrder);
}
