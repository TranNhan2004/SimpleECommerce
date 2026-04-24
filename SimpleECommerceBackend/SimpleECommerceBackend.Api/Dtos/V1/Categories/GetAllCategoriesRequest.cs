using SimpleECommerceBackend.Application.Models.Categories;

namespace SimpleECommerceBackend.Api.Dtos.V1.Categories;

public class GetAllCategoriesRequest
{
    public static GetAllCategoriesQuery ToQuery(GetAllCategoriesRequest request)
    {
        return new GetAllCategoriesQuery();
    }
}