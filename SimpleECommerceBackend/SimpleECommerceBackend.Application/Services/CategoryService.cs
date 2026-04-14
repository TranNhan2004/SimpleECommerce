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

        var categoryKeys = categoryIds.Select(id => CategoryCacheKey.GetCategory.Replace("{id}", id.ToString())).ToList();
        var cachedCategories = (await _cacheService.GetBulkAsync<Category>(categoryKeys)).ToList();

        var missingIds = new List<Guid>();
        var missingIndexes = new List<int>();

        for (int i = 0; i < cachedCategories.Count; i++)
        {
            if (cachedCategories[i] is not null)
            {
                continue;
            }

            missingIds.Add(categoryIds[i]);
            missingIndexes.Add(i);
        }

        if (missingIds.Count == 0)
        {
            return cachedCategories!;
        }

        var missingCategories = await _categoryRepository.FindAllByConditionAsync(
            query => query.Where(category => missingIds.Contains(category.Id)),
            false);

        var missingCategoryMap = new Dictionary<Guid, Category>(missingCategories.Count);
        foreach (var category in missingCategories)
        {
            missingCategoryMap[category.Id] = category;
        }

        foreach (var category in missingCategories)
        {
            var cacheKey = CategoryCacheKey.GetCategory.Replace("{id}", category.Id.ToString());
            await _cacheService.SetAsync(cacheKey, category, TimeSpan.FromMinutes(CategoryCacheKey.GetCategoryTtlMinutes));
        }

        for (int i = 0; i < missingIndexes.Count; i++)
        {
            var index = missingIndexes[i];
            var categoryId = categoryIds[index];

            if (missingCategoryMap.TryGetValue(categoryId, out var category))
            {
                cachedCategories[index] = category;
            }
        }

        return [..cachedCategories
            .Where(category => category is not null)
            .Select(category => category!)];
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