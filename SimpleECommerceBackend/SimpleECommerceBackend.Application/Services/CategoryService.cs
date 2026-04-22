using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;

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

    public Category DeleteCategory(Category category)
    {
        return _categoryRepository.Delete(category);
    }

    public async Task<IReadOnlyList<Category>> GetAllCategoriesAsync()
    {
        var categoryIds = await _cacheService.GetAsync<IReadOnlyList<Guid>>(CategoryCacheKeys.GetAllCategory);

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
                CategoryCacheKeys.GetAllCategory,
                orderedCategories.Select(category => category.Id).ToList(),
                TimeSpan.FromMinutes(CategoryCacheKeys.GetAllCategoryTtlMinutes)
            );
        }

        return orderedCategories;
    }

    public async Task<Category> GetCategoryByIdAsync(Guid id)
    {
        var cacheKey = CategoryCacheKeys.GetCategory.Replace("{id}", id.ToString());
        var cachedCategory = await _cacheService.GetAsync<Category>(cacheKey);

        if (cachedCategory is not null)
        {
            return cachedCategory;
        }

        var category = await _categoryRepository.FindByIdAsync(id)
            ?? throw new ResourceNotFoundException(
                CategoryErrorCodes.NotFoundById,
                $"Category with Id = {id} was not found."
            );

        await _cacheService.SetAsync(cacheKey, category, TimeSpan.FromMinutes(CategoryCacheKeys.GetCategoryTtlMinutes));
        return category;
    }

    public async Task<Category> GetCategoryByIdForUpdateAsync(Guid id)
    {
        var category = await _categoryRepository.FindByIdAsync(id)
           ?? throw new ResourceNotFoundException(
               CategoryErrorCodes.NotFoundById,
               $"Category with Id = {id} was not found."
           );

        return category;
    }

    public Task InvalidateCacheByIdAsync(Guid id)
    {
        return _cacheService.RemoveAsync(CategoryCacheKeys.GetCategory.Replace("{id}", id.ToString()));
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

        await _cacheService.SetAsync(CategoryCacheKeys.GetAllCategory, categoryIds, TimeSpan.FromMinutes(CategoryCacheKeys.GetAllCategoryTtlMinutes));

        return categories;
    }
}
