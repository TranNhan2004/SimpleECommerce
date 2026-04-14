namespace SimpleECommerceBackend.Domain.Constants.CacheKeys;

public static class CategoryCacheKey
{
    public const string GetAllCategory = "Category:List";
    public const string GetCategory = "Category:{id}";

    public const int GetAllCategoryTtlMinutes = 60;
    public const int GetCategoryTtlMinutes = 60;
}