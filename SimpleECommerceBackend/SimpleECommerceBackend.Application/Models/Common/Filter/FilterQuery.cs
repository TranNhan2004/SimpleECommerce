using SimpleECommerceBackend.Application.Models.Common.Sorting;
using SimpleECommerceBackend.Application.Models.Common.Pagination;

namespace SimpleECommerceBackend.Application.Models.Common.Filter;

public abstract class FilterQuery<TEntity> : PaginationQuery where TEntity : class
{
    public FilterGroup? FilterGroup { get; init; }
    public List<FilterCriterion>? FilterCriteria { get; init; }
    public List<SortField>? SortFields { get; init; }

    public abstract FilterQueryMap<TEntity> GetFilterQueryMap();
}
