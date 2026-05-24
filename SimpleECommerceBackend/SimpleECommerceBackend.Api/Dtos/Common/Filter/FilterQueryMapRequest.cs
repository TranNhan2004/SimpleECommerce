using System.Linq.Expressions;

namespace SimpleECommerceBackend.Api.Dtos.Common.Filter;

public sealed class FilterQueryMapRequest<TEntity> where TEntity : class
{
    private readonly Dictionary<string, FilterQueryMapFieldRequest<TEntity>> fields = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, FilterQueryMapFieldRequest<TEntity>> Fields => fields;

    public FilterQueryMapRequest<TEntity> Map<TField>(
        string fieldName,
        Expression<Func<TEntity, TField>> selector
    )
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException("Filter field name cannot be blank.", nameof(fieldName));
        }

        var normalizedFieldName = fieldName.Trim();
        fields[normalizedFieldName] = new FilterQueryMapFieldRequest<TEntity>(
            normalizedFieldName,
            selector,
            typeof(TField)
        );
        return this;
    }

    public bool TryGetField(string fieldName, out FilterQueryMapFieldRequest<TEntity> field)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            field = null!;
            return false;
        }

        return fields.TryGetValue(fieldName.Trim(), out field!);
    }
}
