using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Categories;

namespace SimpleECommerceBackend.Application.UseCases.Categories.Queries;

public class GetAllCategoriesForAdminHandler : IUseCaseHandler<GetAllCategoriesQueryForAdmin, GetAllCategoriesResultForAdmin>
{
    private readonly ICategoryService _categoryService;

    public GetAllCategoriesForAdminHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<GetAllCategoriesResultForAdmin> HandleAsync(
        GetAllCategoriesQueryForAdmin request,
        CancellationToken cancellationToken
    )
    {
        return await _categoryService.GetAllCategoriesForAdminAsync(request);
    }
}