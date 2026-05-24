using SimpleECommerceBackend.Api.Dtos.Common.Filter;
using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Api.Dtos.V1.Categories;

public class GetAllCategoriesRequest : FilterRequest<Category>
{
    public override FilterQueryMapRequest<Category> GetFilterRequestMap()
    {
        return new FilterQueryMapRequest<Category>()
            .Map("id", category => category.Id)
            .Map("name", category => category.Name);
    }

    public static GetAllCategoriesQuery ToQuery(GetAllCategoriesRequest request)
    {
        return new GetAllCategoriesQuery
        {
            CurrentPage = request.CurrentPage,
            ItemsPerPage = request.ItemsPerPage,
            FilterGroup = request.BuildFilterGroup(),
            FilterCriteria = request.BuildFilterCriteria(),
            SortFields = request.BuildSortFields()
        };
    }
}
