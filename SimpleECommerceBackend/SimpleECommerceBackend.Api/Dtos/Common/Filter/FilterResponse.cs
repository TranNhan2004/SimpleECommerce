using SimpleECommerceBackend.Api.Dtos.Common.Pagination;

namespace SimpleECommerceBackend.Api.Dtos.Common.Filter;

public class FilterResponse<T> : PaginationResponse<T> where T : class
{

}