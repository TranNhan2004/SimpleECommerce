using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Categories;

namespace SimpleECommerceBackend.Application.UseCases.Categories.Queries;

public class GetAllCategoriesHandler : IUseCaseHandler<GetAllCategoriesQuery, GetAllCategoriesResult>
{
    private readonly ICategoryService _categoryService;
    private readonly Serilog.ILogger _logger;

    public GetAllCategoriesHandler(ICategoryService categoryService, Serilog.ILogger logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    public async Task<GetAllCategoriesResult> HandleAsync(
        GetAllCategoriesQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _categoryService.GetAllCategoriesAsync(request);
    }
}
