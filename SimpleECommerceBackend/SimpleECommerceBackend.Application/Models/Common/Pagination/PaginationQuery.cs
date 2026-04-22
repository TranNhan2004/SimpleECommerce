
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Api.DTOs.Common.Pagination;

public class PaginationQuery
{
    private int currentPage = 1;
    private int rowsPerPage = int.MaxValue;

    public int CurrentPage
    {
        get => currentPage;
        init
        {
            if (value < 1)
                throw new ValidationException(
                    PaginationErrorCodes.InvalidCurrentPage,
                    "Current page must be a positive integer.",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "CurrentPage"
                    }
                );

            currentPage = value;
        }
    }

    public int RowsPerPage
    {
        get => rowsPerPage;
        init
        {
            if (value < 1)
                throw new ValidationException(
                    PaginationErrorCodes.InvalidRowsPerPage,
                    "Rows per page must be a positive integer.",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "RowsPerPage"
                    }
                );

            rowsPerPage = value;
        }
    }

}