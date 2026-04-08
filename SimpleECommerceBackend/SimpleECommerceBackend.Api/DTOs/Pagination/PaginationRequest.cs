namespace SimpleECommerceBackend.Api.DTOs.Pagination;

public class PaginationRequest
{
    public int CurrentPage { get; init; } = 1;
    public int RowsPerPage { get; init; } = int.MaxValue;
}