using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Application.Models.Pagination;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Extensions;

public static class PaginationExtension
{
    public static async Task<PaginationResultModel<T>> ToPaginationResultModelAsync<T>(
        this IQueryable<T> query,
        PaginationRequestModel paginationRequestModel,
        CancellationToken cancellationToken = default
    ) where T : class
    {
        int currentPage = paginationRequestModel.CurrentPage;
        int rowsPerPage = paginationRequestModel.RowsPerPage;

        int totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((currentPage - 1) * rowsPerPage)
            .Take(rowsPerPage)
            .ToListAsync(cancellationToken);

        return new PaginationResultModel<T>
        {
            Items = items,
            CurrentPage = currentPage,
            RowsPerPage = rowsPerPage,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)rowsPerPage)
        };
    }
}