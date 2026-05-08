using SimpleECommerceBackend.Api.Dtos.Common.Filter;
using SimpleECommerceBackend.Application.Models.Products;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Api.Dtos.V1.Products;

public class GetAllProductsRequestForCustomer : FilterRequest<Product>
{
    public override FilterQueryMapRequest<Product> GetFilterRequestMap()
    {
        return new FilterQueryMapRequest<Product>()
            .Map("id", product => product.Id)
            .Map("name", product => product.Name)
            .Map("description", product => product.Description)
            .Map("categoryId", product => product.CategoryId)
            .Map("sellerId", product => product.SellerId)
            .Map("currentPriceAmount", product => product.CurrentPrice.Amount)
            .Map("currentPriceCurrency", product => product.CurrentPrice.Currency);
    }

    public static GetAllProductsQueryForCustomer ToQuery(GetAllProductsRequestForCustomer request)
    {
        return new GetAllProductsQueryForCustomer
        {
            CurrentPage = request.CurrentPage,
            ItemsPerPage = request.ItemsPerPage,
            FilterGroup = request.BuildFilterGroup(),
            FilterCriteria = request.BuildFilterCriteria(),
            SortFields = request.BuildSortFields()
        };
    }
}