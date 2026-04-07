using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext appDbContext) : base(appDbContext)
    {
    }
}