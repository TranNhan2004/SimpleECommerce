namespace SimpleECommerceBackend.Domain.Constants.CacheKeys;

public static class ProductCacheKeys
{
    public const string GetAllProductsPrefix = "GetAllProducts";
    public const string GetProduct = "Product:{id}";

    public const int GetAllProductsTtlMinutes = 60;
    public const int GetProductTtlMinutes = 60;

    public static string GetAllProductsKey(string hash)
    {
        return $"{GetAllProductsPrefix}:{hash}";
    }

    public static string GetProductKey(Guid id)
    {
        return GetProduct.Replace("{id}", id.ToString());
    }
}