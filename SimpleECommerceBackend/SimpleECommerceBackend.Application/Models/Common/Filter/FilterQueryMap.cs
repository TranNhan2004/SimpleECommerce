using System.Linq.Expressions;

namespace SimpleECommerceBackend.Application.Models.Common.Filter;

public sealed class FilterQueryMap<TEntity> where TEntity : class
{
    private readonly Dictionary<string, FilterQueryMapField<TEntity>> fields =
        new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, FilterQueryMapField<TEntity>> Fields => fields;

    public FilterQueryMap<TEntity> Map<TField>(
        string fieldName,
        Expression<Func<TEntity, TField>> selector
    )
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException("Filter field name cannot be blank.", nameof(fieldName));
        }

        var normalizedFieldName = fieldName.Trim();
        fields[normalizedFieldName] = new FilterQueryMapField<TEntity>(
            normalizedFieldName,
            selector,
            typeof(TField)
        );
        return this;
    }

    public bool TryGetField(string fieldName, out FilterQueryMapField<TEntity> field)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            field = null!;
            return false;
        }

        return fields.TryGetValue(fieldName.Trim(), out field!);
    }
}
