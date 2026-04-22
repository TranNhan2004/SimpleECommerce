using SimpleECommerceBackend.Api.DTOs.Common.Pagination;

namespace SimpleECommerceBackend.Api.DTOs.Common.Filter;

public class FilterResult<T> : PaginationResponse<T> where T : class
{

}