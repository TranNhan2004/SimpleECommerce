namespace SimpleECommerceBackend.Application.Models.Pagination;

public class PaginationResultModel<T> where T : class
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int CurrentPage { get; init; }
    public int RowsPerPage { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
}