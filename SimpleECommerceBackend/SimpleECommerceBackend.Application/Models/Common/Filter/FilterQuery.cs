using SimpleECommerceBackend.Api.DTOs.Common.Sorting;
using SimpleECommerceBackend.Api.DTOs.Common.Pagination;

namespace SimpleECommerceBackend.Api.DTOs.Common.Filter;

public abstract class FilterQuery<TEntity> : PaginationQuery where TEntity : class
{
    public FilterGroup? FilterGroup { get; init; }
    public List<FilterCriterion>? FilterCriteria { get; init; }
    public List<SortField>? SortFields { get; init; }

    public abstract FilterQueryMap<TEntity> GetFilterQueryMap();
}
