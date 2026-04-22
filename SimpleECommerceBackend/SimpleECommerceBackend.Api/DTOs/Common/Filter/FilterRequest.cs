using SimpleECommerceBackend.Api.DTOs.Common.Sorting;
using SimpleECommerceBackend.Api.DTOs.Common.Pagination;

namespace SimpleECommerceBackend.Api.DTOs.Common.Filter;

public abstract class FilterRequest<TEntity> : PaginationRequest where TEntity : class
{
    public FilterGroupRequest? FilterGroup { get; init; }
    public List<FilterCriterionRequest>? FilterCriteria { get; init; }
    public List<SortFieldRequest>? SortFields { get; init; }

    public abstract FilterQueryMapRequest<TEntity> GetFilterRequestMap();
}
