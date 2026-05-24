
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.Models.Common.Pagination;

public class PaginationQuery
{
    private int currentPage = 1;
    private int itemsPerPage = int.MaxValue;

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

    public int ItemsPerPage
    {
        get => itemsPerPage;
        init
        {
            if (value < 1)
                throw new ValidationException(
                    PaginationErrorCodes.InvalidItemsPerPage,
                    "Items per page must be a positive integer.",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "ItemsPerPage"
                    }
                );

            itemsPerPage = value;
        }
    }

}