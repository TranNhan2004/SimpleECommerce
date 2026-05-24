using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Domain.Constants.CacheKeys;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.Services.Business;

public class CategoryService : ServiceBase, ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICacheService cacheService, ICategoryRepository categoryRepository)
        : base(cacheService)
    {
        _categoryRepository = categoryRepository;
    }

    public Category CreateCategory(Category category)
    {
        return _categoryRepository.Add(category);
    }

    public Category DeleteCategory(Category category)
    {
        return _categoryRepository.Delete(category);
    }

    public async Task<GetAllCategoriesResult> GetAllCategoriesAsync(GetAllCategoriesQuery query)
    {
        var cacheKey = CategoryCacheKeys.GetAllCategoriesKey(query.GetContentHash());
        var cachedResult = await CacheService.GetAsync<GetAllCategoriesResult>(cacheKey);

        if (cachedResult is not null)
        {
            return cachedResult;
        }

        var categories = await _categoryRepository.FindAllWithFilterAsync(query);
        var result = GetAllCategoriesResult.FromFilterResult(categories);

        await CacheService.SetAsync(
            cacheKey,
            result,
            TimeSpan.FromMinutes(CategoryCacheKeys.GetAllCategoriesTtlMinutes)
        );

        return result;
    }

    public async Task<GetAllCategoriesResultForAdmin> GetAllCategoriesForAdminAsync(GetAllCategoriesQueryForAdmin query)
    {
        var cacheKey = CategoryCacheKeys.GetAllCategoriesForAdminKey(query.GetContentHash());
        var cachedResult = await CacheService.GetAsync<GetAllCategoriesResultForAdmin>(cacheKey);

        if (cachedResult is not null)
        {
            return cachedResult;
        }

        var categories = await _categoryRepository.FindAllForAdminWithFilterAsync(query);
        var result = GetAllCategoriesResultForAdmin.FromFilterResult(categories);

        await CacheService.SetAsync(
            cacheKey,
            result,
            TimeSpan.FromMinutes(CategoryCacheKeys.GetAllCategoriesForAdminTtlMinutes)
        );

        return result;
    }

    public async Task<Category> GetCategoryByIdAsync(Guid id)
    {
        var cacheKey = CategoryCacheKeys.GetCategoryKey(id);
        var cachedCategory = await CacheService.GetAsync<Category>(cacheKey);

        if (cachedCategory is not null)
        {
            return cachedCategory;
        }

        var category = await _categoryRepository.FindByIdAsync(id)
            ?? throw new ResourceNotFoundException(
                CategoryErrorCodes.NotFoundById,
                $"Category with Id = {id} was not found."
            );

        await CacheService.SetAsync(cacheKey, category, TimeSpan.FromMinutes(CategoryCacheKeys.GetCategoryTtlMinutes));
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
}
