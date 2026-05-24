using SimpleECommerceBackend.Application.Models.Common.Filter;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Models.Products;

public class GetAllProductsQueryForCustomer : FilterQuery<Product>
{
    public override FilterQueryMap<Product> GetFilterQueryMap()
    {
        return new FilterQueryMap<Product>()
            .Map("id", product => product.Id)
            .Map("name", product => product.Name)
            .Map("description", product => product.Description)
            .Map("categoryId", product => product.CategoryId)
            .Map("sellerId", product => product.SellerId)
            .Map("averageRating", product => product.AverageRating)
            .Map("totalRatings", product => product.TotalRatings);
    }
}
