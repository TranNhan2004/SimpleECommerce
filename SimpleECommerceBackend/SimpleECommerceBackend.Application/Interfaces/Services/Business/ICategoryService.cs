using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Interfaces.Services.Business;

public interface ICategoryService : ICacheConsumingService
{
    Category CreateCategory(Category category);
    Task<IReadOnlyList<Category>> GetAllCategoriesAsync();
}