using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Services.Business;

public interface ICategoryService : ICacheConsumingService
{
    Category CreateCategory(Category category);
    Task<IReadOnlyList<Category>> GetAllCategoriesAsync();
    Task<Category> GetCategoryByIdAsync(Guid id);
    Task<Category> GetCategoryByIdForUpdateAsync(Guid id);
    Category DeleteCategory(Category category);
}