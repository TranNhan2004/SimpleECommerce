namespace SimpleECommerceBackend.Domain.Constants.CacheKeys;

public static class CategoryCacheKeys
{
    public const string GetAllCategoriesPrefix = "GetAllCategories";
    public const string GetCategory = "Category:{id}";
    public const string GetAllCategoriesForAdminPrefix = "GetAllCategoriesForAdmin";

    public const int GetAllCategoriesTtlMinutes = 60;
    public const int GetCategoryTtlMinutes = 60;
    public const int GetAllCategoriesForAdminTtlMinutes = 60;

    public static string GetAllCategoriesKey(string hash)
    {
        return $"{GetAllCategoriesPrefix}:{hash}";
    }

    public static string GetAllCategoriesForAdminKey(string hash)
    {
        return $"{GetAllCategoriesForAdminPrefix}:{hash}";
    }

    public static string GetCategoryKey(Guid id)
    {
        return GetCategory.Replace("{id}", id.ToString());
    }
}