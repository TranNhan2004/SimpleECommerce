namespace SimpleECommerceBackend.Application.Models.Categories;

public class GetAllCategoriesRequest
{
    public static GetAllCategoriesQuery ToQuery(GetAllCategoriesRequest request)
    {
        return new GetAllCategoriesQuery();
    }
}