using System.Linq.Expressions;

namespace SimpleECommerceBackend.Api.Dtos.Common.Filter;

public sealed record FilterQueryMapFieldRequest<TEntity>(
    string FieldName,
    LambdaExpression Selector,
    Type FieldType
) where TEntity : class;
