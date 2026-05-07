using SimpleECommerceBackend.Application.Utils;
using SimpleECommerceBackend.Api.Dtos.Common.Pagination;
using SimpleECommerceBackend.Api.Dtos.Common.Sorting;
using SimpleECommerceBackend.Application.Models.Common.Filter;
using SimpleECommerceBackend.Application.Models.Common.Sorting;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Exceptions;

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

    public List<FilterCriterion>? BuildFilterCriteria()
    {
        if (FilterCriteria is null)
        {
            return null;
        }

        var filterRequestMap = GetFilterRequestMap();

        return [..FilterCriteria
            .Select(criterion =>
            {
                if (!filterRequestMap.TryGetField(criterion.FieldName, out _))
                {
                    throw new ValidationException(
                        FilterErrorCodes.UnknownField,
                        $"Unknown filter field '{criterion.FieldName}'.",
                        new Dictionary<string, object?>
                        {
                            ["fieldName"] = criterion.FieldName
                        }
                    );
                }

                return FilterCriterionRequest.ToModel(criterion);
            })];
    }

    public List<SortField>? BuildSortFields()
    {
        if (SortFields is null)
        {
            return null;
        }

        var filterRequestMap = GetFilterRequestMap();

        return [..SortFields
            .Select(sortField =>
            {
                if (!filterRequestMap.TryGetField(sortField.FieldName, out _))
                {
                    throw new ValidationException(
                        FilterErrorCodes.UnknownField,
                        $"Unknown sort field '{sortField.FieldName}'.",
                        new Dictionary<string, object?>
                        {
                            ["fieldName"] = sortField.FieldName
                        }
                    );
                }

                return new SortField
                {
                    FieldName = sortField.FieldName,
                    IsAscending = sortField.IsAscending
                };
            })];
    }
}
