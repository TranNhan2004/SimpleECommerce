using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Services;

[AutoConstructor]
public partial class CategoryService : ICategoryService
{
    private readonly ICacheService _cacheService;
    private readonly ICategoryRepository _categoryRepository;

    public async Task<IReadOnlyCollection<Category>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.FindAllAsync();
    }
}