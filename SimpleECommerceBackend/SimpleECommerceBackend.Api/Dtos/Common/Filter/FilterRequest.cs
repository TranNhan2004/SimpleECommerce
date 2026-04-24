using SimpleECommerceBackend.Application.Utils;
using SimpleECommerceBackend.Api.Dtos.Common.Pagination;
using SimpleECommerceBackend.Api.Dtos.Common.Sorting;
using SimpleECommerceBackend.Application.Models.Common.Filter;

namespace SimpleECommerceBackend.Api.Dtos.Common.Filter;

public abstract class FilterRequest<TEntity> : PaginationRequest where TEntity : class
{
    public string? GroupPattern { get; init; }
    public List<FilterCriterionRequest>? FilterCriteria { get; init; }
    public List<SortFieldRequest>? SortFields { get; init; }

    public abstract FilterQueryMapRequest<TEntity> GetFilterRequestMap();

    public FilterGroup? BuildFilterGroup()
    {
        return FilterGroupConverter.Build(GroupPattern, FilterCriteria?.Count ?? 0);
    }
}
