using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Categories;

namespace SimpleECommerceBackend.Application.UseCases.Categories.Queries;

public class GetCategoryHandler : IUseCaseHandler<GetCategoryQuery, GetCategoryResult>
{
    private readonly Serilog.ILogger _logger;
    private readonly ICategoryService _categoryService;

    public GetCategoryHandler(Serilog.ILogger logger, ICategoryService categoryService)
    {
        _logger = logger;
        _categoryService = categoryService;
    }

    public async Task<GetCategoryResult> HandleAsync(
        GetCategoryQuery request,
        CancellationToken cancellationToken
    )
    {
        var category = await _categoryService.GetCategoryByIdAsync(request.Id);
        return GetCategoryResult.FromEntity(category);
    }
}
