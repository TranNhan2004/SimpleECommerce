using System.Linq.Expressions;

namespace SimpleECommerceBackend.Api.DTOs.Common.Filter;

public sealed record FilterQueryMapField<TEntity>(
    string FieldName,
    LambdaExpression Selector,
    Type FieldType
) where TEntity : class;
