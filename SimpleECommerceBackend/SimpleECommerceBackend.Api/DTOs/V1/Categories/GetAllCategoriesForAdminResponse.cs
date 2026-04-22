using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.Models.Categories;

public class GetAllCategoriesForAdminResponse
{
    public class CategorySummary
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public string Status { get; init; } = null!;
        public Guid AdminId { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public DateTimeOffset? UpdatedAt { get; init; }
    }

    public IReadOnlyList<CategorySummary> Items { get; init; } = [];
    public int CurrentPage { get; init; }
    public int RowsPerPage { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
    public bool HasPreviousPage { get; init; }
    public bool HasNextPage { get; init; }

    public static GetAllCategoriesForAdminResponse FromResult(GetAllCategoriesResult result)
    {
        return new GetAllCategoriesForAdminResponse
        {
            Items = [..result.Items
                .Select(item => new CategorySummary
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
            RowsPerPage = result.RowsPerPage,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages,
            HasPreviousPage = result.HasPreviousPage,
            HasNextPage = result.HasNextPage
        };
    }
}
