using SimpleECommerceBackend.Api.Dtos.Common.Filter;
using SimpleECommerceBackend.Application.Models.Categories;

namespace SimpleECommerceBackend.Api.Dtos.V1.Categories;

public class CategoryItem
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}

public class GetAllCategoriesResponse : FilterResponse<CategoryItem>
{
    public static GetAllCategoriesResponse FromResult(GetAllCategoriesResult result)
    {
        return new GetAllCategoriesResponse
        {
            Items = [..result.Items
                .Select(item => new CategoryItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description
                })],
            CurrentPage = result.CurrentPage,
            ItemsPerPage = result.ItemsPerPage,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages
        };
    }
}
