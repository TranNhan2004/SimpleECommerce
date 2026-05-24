using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Categories;

namespace SimpleECommerceBackend.Application.UseCases.Categories.Queries;

public class GetAllCategoriesHandler : IUseCaseHandler<GetAllCategoriesQuery, GetAllCategoriesResult>
{
    private readonly ICategoryService _categoryService;

    public GetAllCategoriesHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<GetAllCategoriesResult> HandleAsync(
        GetAllCategoriesQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _categoryService.GetAllCategoriesAsync(request);
    }
}
