using SimpleECommerceBackend.Application.Models.Common.Filter;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Application.Models.Categories;

public class CategoryItem
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public CategoryStatus Status { get; init; }
    public Guid AdminId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}

public class GetAllCategoriesResult : FilterResult<CategoryItem>
{
    public static GetAllCategoriesResult FromEntities(IReadOnlyList<Category> entities)
    {
        var items = entities
            .Select(entity => new CategoryItem
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Status = entity.Status,
                AdminId = entity.AdminId,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            })
            .ToList();

        return new GetAllCategoriesResult
        {
            Items = items,
            CurrentPage = 1,
            RowsPerPage = items.Count == 0 ? 1 : items.Count,
            TotalItems = items.Count,
            TotalPages = items.Count == 0 ? 0 : 1
        };
    }

    public static GetAllCategoriesResult FromFilterResult(FilterResult<CategoryItem> filterResult)
    {
        return new GetAllCategoriesResult
        {
            Items = filterResult.Items,
            CurrentPage = filterResult.CurrentPage,
            RowsPerPage = filterResult.RowsPerPage,
            TotalItems = filterResult.TotalItems,
            TotalPages = filterResult.TotalPages
        };
    }
}
