using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Services.Business;

public interface ICategoryService : ICacheConsumingService
{
    Category CreateCategory(Category category);
    Task<GetAllCategoriesResult> GetAllCategoriesAsync(GetAllCategoriesQuery query);
    Task<GetAllCategoriesResultForAdmin> GetAllCategoriesForAdminAsync(GetAllCategoriesQueryForAdmin query);
    Task<Category> GetCategoryByIdAsync(Guid id);
    Task<Category> GetCategoryByIdForUpdateAsync(Guid id);
    Category DeleteCategory(Category category);
}