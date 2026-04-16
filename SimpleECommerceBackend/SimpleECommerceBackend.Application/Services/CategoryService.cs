using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Services;

[AutoConstructor]
public partial class CategoryService : ICategoryService
{
    private readonly ICacheService _cacheService;
    private readonly ICategoryRepository _categoryRepository;

    public Category CreateCategory(Category category)
    {
        return _categoryRepository.Add(category);
    }

    public async Task<IReadOnlyList<Category>> GetAllCategoriesAsync()
    {
        var categoryIds = await _cacheService.GetAsync<IReadOnlyList<Guid>>(CategoryCacheKey.GetAllCategory);

        if (categoryIds is null || categoryIds.Count == 0)
        {
            return await LoadAllCategoriesFromDatabaseAndCacheIdsAsync();
        }

        var categories = await _categoryRepository.FindAllByConditionAsync(
            query => query.Where(category => categoryIds.Contains(category.Id)),
            false
        );

        if (categories.Count == 0)
            return await LoadAllCategoriesFromDatabaseAndCacheIdsAsync();

        var categoriesById = categories.ToDictionary(category => category.Id);
        var orderedCategories = categoryIds
            .Where(categoriesById.ContainsKey)
            .Select(id => categoriesById[id])
            .ToList();

        if (orderedCategories.Count != categoryIds.Count)
        {
            await _cacheService.SetAsync(
                CategoryCacheKey.GetAllCategory,
                orderedCategories.Select(category => category.Id).ToList(),
                TimeSpan.FromMinutes(CategoryCacheKey.GetAllCategoryTtlMinutes)
            );
        }

        return orderedCategories;
    }

    public Task InvalidateCacheByIdAsync(Guid id)
    {
        return _cacheService.RemoveAsync(CategoryCacheKey.GetCategory.Replace("{id}", id.ToString()));
    }

    public Task InvalidateCacheByKeyAsync(string key)
    {
        return _cacheService.RemoveAsync(key);
    }

    private async Task<IReadOnlyList<Category>> LoadAllCategoriesFromDatabaseAndCacheIdsAsync()
    {
        var categories = await _categoryRepository.FindAllAsync();

        var categoryIds = new List<Guid>(categories.Count);
        foreach (var category in categories)
        {
            categoryIds.Add(category.Id);
        }

        await _cacheService.SetAsync(CategoryCacheKey.GetAllCategory, categoryIds, TimeSpan.FromMinutes(CategoryCacheKey.GetAllCategoryTtlMinutes));

        return categories;
    }
}
