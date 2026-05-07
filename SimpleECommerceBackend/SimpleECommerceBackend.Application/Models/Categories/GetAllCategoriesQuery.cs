using SimpleECommerceBackend.Application.Models.Common.Filter;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Application.Models.Categories;

public class GetAllCategoriesQuery : FilterQuery<Category>
{
    public override FilterQueryMap<Category> GetFilterQueryMap()
    {
        return new FilterQueryMap<Category>()
            .Map("id", category => category.Id)
            .Map("name", category => category.Name);
    }
}
