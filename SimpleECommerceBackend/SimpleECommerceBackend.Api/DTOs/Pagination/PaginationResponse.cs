namespace SimpleECommerceBackend.Api.DTOs.Pagination;

public class PaginationResponse<T> where T : class
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int CurrentPage { get; init; }
    public int RowsPerPage { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
}