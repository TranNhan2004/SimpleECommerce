using Mapster;
using SimpleECommerceBackend.Api.DTOs.V1.Categories;
using SimpleECommerceBackend.Application.Models.Categories;

namespace SimpleECommerceBackend.Api.Mappers;

public class CategoryMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<GetAllCategoriesRequest, GetAllCategoriesQuery>();
        config.NewConfig<GetAllCategoriesResult, GetAllCategoriesResponse>();
        config.NewConfig<GetAllCategoriesResult, GetAllCategoriesForAdminResponse>();

        config.NewConfig<CreateCategoryRequest, CreateCategoryCommand>();
        config.NewConfig<CreateCategoryResult, CreateCategoryResponse>();
    }
}