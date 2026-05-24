using System.Linq.Expressions;

namespace SimpleECommerceBackend.Application.Models.Common.Filter;

public sealed record FilterQueryMapField<TEntity>(
    string FieldName,
    LambdaExpression Selector,
    Type FieldType
) where TEntity : class;
