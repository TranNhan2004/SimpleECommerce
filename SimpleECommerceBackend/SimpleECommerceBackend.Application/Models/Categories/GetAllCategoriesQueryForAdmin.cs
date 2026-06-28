using SimpleECommerceBackend.Application.Models.Common.Filter;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Models.Categories;

public class GetAllCategoriesQueryForAdmin : FilterQuery<Category>
{
    public override FilterQueryMap<Category> GetFilterQueryMap()
    {
        return new FilterQueryMap<Category>()
            .Map("id", category => category.Id)
            .Map("name", category => category.Name)
            .Map("description", category => category.Description)
            .Map("status", category => category.Status)
            .Map("createdAt", category => category.CreatedAt)
            .Map("updatedAt", category => category.UpdatedAt)
            .Map("createdById", category => category.CreatedById)
            .Map("updatedById", category => category.UpdatedById);
    }
}
