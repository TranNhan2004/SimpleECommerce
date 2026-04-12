using Mapster;
using SimpleECommerceBackend.Api.DTOs.V1_0.Category;
using SimpleECommerceBackend.Application.Models.Categories;

namespace SimpleECommerceBackend.Api.Mappers;

public class CategoryMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<GetAllCategoriesRequest, GetAllCategoriesQuery>();
        config.NewConfig<GetAllCategoriesResult, GetAllCategoriesResponse>();
        config.NewConfig<GetAllCategoriesResult, GetAllCategoriesForAdminResponse>();
    }
}