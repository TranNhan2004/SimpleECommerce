using SimpleECommerceBackend.Application.Models.Common.Filter;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;

namespace SimpleECommerceBackend.Application.Models.Categories;

public class CategoryItemForAdmin
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public CategoryStatus Status { get; init; }
    public Guid AdminId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }

    public static CategoryItemForAdmin FromEntity(Category entity)
    {
        return new CategoryItemForAdmin
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Status = entity.Status,
            AdminId = entity.AdminId,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

public class GetAllCategoriesResultForAdmin : FilterResult<CategoryItemForAdmin>
{
    public static GetAllCategoriesResultForAdmin FromFilterResult(FilterResult<CategoryItemForAdmin> filterResult)
    {
        return new GetAllCategoriesResultForAdmin
        {
            Items = filterResult.Items,
            CurrentPage = filterResult.CurrentPage,
            ItemsPerPage = filterResult.ItemsPerPage,
            TotalItems = filterResult.TotalItems,
            TotalPages = filterResult.TotalPages
        };
    }
}
