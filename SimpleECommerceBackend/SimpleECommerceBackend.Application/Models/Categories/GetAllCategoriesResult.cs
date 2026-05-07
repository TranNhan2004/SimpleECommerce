using SimpleECommerceBackend.Application.Models.Common.Filter;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Application.Models.Categories;

public class CategoryItem
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }

    public static CategoryItem FromEntity(Category entity)
    {
        return new CategoryItem
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description
        };
    }
}

public class GetAllCategoriesResult : FilterResult<CategoryItem>
{
    public static GetAllCategoriesResult FromFilterResult(FilterResult<CategoryItem> filterResult)
    {
        return new GetAllCategoriesResult
        {
            Items = filterResult.Items,
            CurrentPage = filterResult.CurrentPage,
            ItemsPerPage = filterResult.ItemsPerPage,
            TotalItems = filterResult.TotalItems,
            TotalPages = filterResult.TotalPages
        };
    }
}
