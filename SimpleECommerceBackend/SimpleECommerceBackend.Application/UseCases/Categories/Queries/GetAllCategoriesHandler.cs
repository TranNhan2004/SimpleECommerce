using SimpleECommerceBackend.Application.Interfaces.Services.Business;
using SimpleECommerceBackend.Application.Interfaces.UseCases;
using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.UseCases.Categories.Queries;

[AutoConstructor]
public partial class GetAllCategoriesHandler : IUseCaseHandler<GetAllCategoriesQuery, IReadOnlyList<GetAllCategoriesResult>>
{
    private readonly ICategoryService _categoryService;

    public async Task<IReadOnlyList<GetAllCategoriesResult>> HandleAsync(
        GetAllCategoriesQuery request,
        CancellationToken cancellationToken
    )
    {
        var categories = await _categoryService.GetAllCategoriesAsync();

        return [..categories.Select(c => new GetAllCategoriesResult
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            Status = CategoryStatusUtils.ToName(c.Status),
            AdminId = c.AdminId,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        })];
    }
}