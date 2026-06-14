using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Application.Models.Common.Filter;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Infrastructure.Extensions;
using SimpleECommerceBackend.Infrastructure.Persistence;

namespace SimpleECommerceBackend.Infrastructure.Repositories.Business;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    private readonly Serilog.ILogger _logger;
    public CategoryRepository(AppDbContext appDbContext, Serilog.ILogger logger) : base(appDbContext)
    {
        _logger = logger;
    }

    public async Task<FilterResult<CategoryItemForAdmin>> FindAllForAdminWithFilterAsync(FilterQuery<Category> query)
    {
        _logger.Debug("CategoryRepository: entered FindAllForAdminWithFilterAsync");
        return await QueryAll()
            .ToFilterResultAsync(query, e => CategoryItemForAdmin.FromEntity(e));
    }

    public async Task<FilterResult<CategoryItem>> FindAllWithFilterAsync(FilterQuery<Category> query)
    {
        _logger.Debug("CategoryRepository: entered FindAllWithFilterAsync");
        return await QueryAllByCondition(e => e.Where(e => e.Status == CategoryStatus.Active))
            .ToFilterResultAsync(query, e => CategoryItem.FromEntity(e));
    }
}