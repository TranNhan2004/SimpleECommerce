using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

public class ProductVariantPriceRepository : GenericRepository<ProductVariantPrice>, IProductVariantPriceRepository
{
    public ProductVariantPriceRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}
