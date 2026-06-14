using SimpleECommerceBackend.Application.Models.Common.Sorting;
using SimpleECommerceBackend.Application.Models.Common.Pagination;
using System.Text.Json;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.Models.Common.Filter;

public abstract class FilterQuery<TEntity> : PaginationQuery where TEntity : class
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public FilterGroup? FilterGroup { get; init; }
    public List<FilterCriterion>? FilterCriteria { get; init; }
    public List<SortField>? SortFields { get; init; }

    public abstract FilterQueryMap<TEntity> GetFilterQueryMap();

    public virtual string GetContentHash()
    {
        Console.WriteLine("FilterQuery: computing content hash");
        var json = JsonSerializer.Serialize(this, GetType(), JsonOptions);
        return Sha256Utils.ComputeHexHash(json);
    }
}
