using SimpleECommerceBackend.Api.Dtos.Common.Filter;
using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.Dtos.V1.Categories;


public class CategoryItemForAdmin
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string Status { get; init; } = null!;
    public Guid AdminId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}

public class GetAllCategoriesForAdminResponse : FilterResponse<CategoryItemForAdmin>
{
    public static GetAllCategoriesForAdminResponse FromResult(GetAllCategoriesResultForAdmin result)
    {
        return new GetAllCategoriesForAdminResponse
        {
            Items = [..result.Items
                .Select(item => new CategoryItemForAdmin
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Status = EnumUtils.ToDisplayValue(item.Status),
                    AdminId = item.AdminId,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt
                })],
            CurrentPage = result.CurrentPage,
            ItemsPerPage = result.ItemsPerPage,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages
        };
    }
}
