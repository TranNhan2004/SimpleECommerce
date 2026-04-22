using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.UseCases.Categories.Queries;

[AutoConstructor]
public partial class GetAllCategoriesHandler : IUseCaseHandler<GetAllCategoriesQuery, GetAllCategoriesResult>
{
    private readonly ICategoryService _categoryService;

    public async Task<GetAllCategoriesResult> HandleAsync(
        GetAllCategoriesQuery request,
        CancellationToken cancellationToken
    )
    {
        var categories = await _categoryService.GetAllCategoriesAsync();

        return GetAllCategoriesResult.FromEntities(categories);
    }
}
