using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Application.Models.Common.Filter;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Repositories.Business;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<FilterResult<CategoryItem>> FindAllWithFilterAsync(FilterQuery<Category> query);
    Task<FilterResult<CategoryItemForAdmin>> FindAllForAdminWithFilterAsync(FilterQuery<Category> query);
}