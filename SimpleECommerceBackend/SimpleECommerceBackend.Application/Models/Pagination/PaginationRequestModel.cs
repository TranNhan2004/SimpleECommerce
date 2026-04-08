
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.Models.Pagination;

public class PaginationRequestModel
{
    public int CurrentPage { get; init; }
    public int RowsPerPage { get; init; }

    public PaginationRequestModel(int currentPage, int rowsPerPage)
    {
        if (currentPage < 1)
            throw new ValidationException(
                PaginationErrorCode.InvalidCurrentPage,
                "Current page must be a positive integer.",
                new Dictionary<string, object?>
                {
                    ["field"] = "CurrentPage"
                }
            );

        if (rowsPerPage < 1)
            throw new ValidationException(
                PaginationErrorCode.InvalidRowsPerPage,
                "Rows per page must be a positive integer.",
                new Dictionary<string, object?>
                {
                    ["field"] = "RowsPerPage"
                }
            );


        CurrentPage = currentPage;
        RowsPerPage = rowsPerPage;
    }

}