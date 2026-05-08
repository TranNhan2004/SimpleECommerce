using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

public class ProductPriceRepository : GenericRepository<ProductPrice>, IProductPriceRepository
{
    public ProductPriceRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}