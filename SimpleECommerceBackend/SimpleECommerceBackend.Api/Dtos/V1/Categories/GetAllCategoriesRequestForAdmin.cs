using SimpleECommerceBackend.Api.Dtos.Common.Filter;
using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Api.Dtos.V1.Categories;

public class GetAllCategoriesRequestForAdmin : FilterRequest<Category>
{
    public override FilterQueryMapRequest<Category> GetFilterRequestMap()
    {
        return new FilterQueryMapRequest<Category>()
            .Map("id", category => category.Id)
            .Map("name", category => category.Name)
            .Map("description", category => category.Description)
            .Map("status", category => category.Status)
            .Map("createdAt", category => category.CreatedAt)
            .Map("updatedAt", category => category.UpdatedAt)
            .Map("createdById", category => category.CreatedById)
            .Map("updatedById", category => category.UpdatedById);
    }

    public static GetAllCategoriesQueryForAdmin ToQuery(GetAllCategoriesRequestForAdmin request)
    {
        return new GetAllCategoriesQueryForAdmin
        {
            CurrentPage = request.CurrentPage,
            ItemsPerPage = request.ItemsPerPage,
            FilterGroup = request.BuildFilterGroup(),
            FilterCriteria = request.BuildFilterCriteria(),
            SortFields = request.BuildSortFields()
        };
    }
}
