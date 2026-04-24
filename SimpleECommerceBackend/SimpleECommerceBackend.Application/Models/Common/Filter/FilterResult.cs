using SimpleECommerceBackend.Application.Models.Common.Pagination;

namespace SimpleECommerceBackend.Application.Models.Common.Filter;

public class FilterResult<T> : PaginationResult<T> where T : class
{

}