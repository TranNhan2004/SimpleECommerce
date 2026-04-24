using SimpleECommerceBackend.Application.Models.Categories;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Api.Dtos.V1.Categories;

public class GetAllCategoriesResponse
{
    public class CategorySummary
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public string Status { get; init; } = null!;
    }

    public IReadOnlyList<CategorySummary> Items { get; init; } = [];
    public int CurrentPage { get; init; }
    public int RowsPerPage { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
    public bool HasPreviousPage { get; init; }
    public bool HasNextPage { get; init; }

    public static GetAllCategoriesResponse FromResult(GetAllCategoriesResult result)
    {
        return new GetAllCategoriesResponse
        {
            Items = result.Items
                .Select(item => new CategorySummary
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    Status = EnumUtils.ToDisplayValue(item.Status)
                })
                .ToList(),
            CurrentPage = result.CurrentPage,
            RowsPerPage = result.RowsPerPage,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages,
            HasPreviousPage = result.HasPreviousPage,
            HasNextPage = result.HasNextPage
        };
    }
}
