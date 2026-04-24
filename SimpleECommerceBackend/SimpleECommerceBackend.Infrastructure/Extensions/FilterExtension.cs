using System.Globalization;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Application.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;
using SimpleECommerceBackend.Application.Models.Common.Filter;

namespace SimpleECommerceBackend.Infrastructure.Extensions;

public static class FilterExtension
{
    // Runs the pipeline without projection and returns the original entity type.
    public static Task<FilterResult<TEntity>> ToFilterResultAsync<TEntity>(
        this IQueryable<TEntity> query,
        FilterQuery<TEntity> filterQuery,
        CancellationToken cancellationToken = default
    ) where TEntity : class
    {
        return query.ToFilterResultAsync(filterQuery, static entity => entity, cancellationToken);
    }

    // Runs filtering, sorting, pagination, and projection in one reusable entry point.
    public static async Task<FilterResult<TResult>> ToFilterResultAsync<TEntity, TResult>(
        this IQueryable<TEntity> query,
        FilterQuery<TEntity> filterQuery,
        Expression<Func<TEntity, TResult>> selector,
        CancellationToken cancellationToken = default
    )
        where TEntity : class
        where TResult : class
    {
        var mappedQuery = FilterExpression.ApplyFiltering(query, filterQuery);
        mappedQuery = SortingExpression.ApplySorting(mappedQuery, filterQuery);

        return await PaginationExpression.ToFilterResultAsync(
            mappedQuery,
            filterQuery,
            selector,
            cancellationToken
        );
    }

    private static class FilterExpression
    {
        // Converts the request filter payload into one LINQ Where clause.
        public static IQueryable<TEntity> ApplyFiltering<TEntity>(
            IQueryable<TEntity> query,
            FilterQuery<TEntity> filterQuery
        ) where TEntity : class
        {
            var criteria = filterQuery.FilterCriteria ?? [];
            if (criteria.Count == 0)
            {
                return query;
            }

            var filterQueryMap = filterQuery.GetFilterQueryMap();
            var criterionExpressions = criteria
                .Select(criterion => BuildCriterionExpression(criterion, filterQueryMap))
                .ToList();

            var predicate = BuildGroupExpression(
                GetEffectiveFilterGroup(filterQuery.FilterGroup, criterionExpressions.Count),
                criterionExpressions
            );
            return query.Where(predicate);
        }

        // Uses the tree produced by FilterGroupConverter when present, otherwise falls back to a simple AND group.
        private static FilterGroup GetEffectiveFilterGroup(
            FilterGroup? filterGroup,
            int criterionCount
        )
        {
            if (filterGroup is null || filterGroup.Children.Count == 0)
            {
                return CreateAndGroup(criterionCount);
            }

            return filterGroup;
        }

        // Creates the fallback AND group used when no normalized tree was attached to the backend query.
        private static FilterGroup CreateAndGroup(int criterionCount)
        {
            var children = new List<FilterGroupNode>(criterionCount);
            for (var index = 0; index < criterionCount; index++)
            {
                children.Add(
                    new FilterGroupNode
                    {
                        CriterionIndex = index
                    }
                );
            }

            return new FilterGroup
            {
                Logic = FilterGroupLogic.And,
                Children = children
            };
        }

        // Recursively converts a filter group into one boolean expression tree.
        private static Expression<Func<TEntity, bool>> BuildGroupExpression<TEntity>(
            FilterGroup group,
            IReadOnlyList<Expression<Func<TEntity, bool>>> criterionExpressions
        ) where TEntity : class
        {
            if (group.Children.Count == 0)
            {
                throw new ValidationException(
                    FilterErrorCodes.InvalidGroup,
                    "Filter group must contain at least one child."
                );
            }

            var logic = group.Logic == default ? FilterGroupLogic.And : group.Logic;
            var children = group.Children
                .Select(child => BuildGroupNodeExpression<TEntity>(child, criterionExpressions))
                .ToList();

            return logic switch
            {
                FilterGroupLogic.And => CombineExpressions(children, Expression.AndAlso),
                FilterGroupLogic.Or => CombineExpressions(children, Expression.OrElse),
                FilterGroupLogic.Not => BuildNotExpression(children),
                _ => throw new ValidationException(
                    FilterErrorCodes.UnsupportedGroupLogic,
                    $"Unsupported filter group logic '{group.Logic}'.",
                    new Dictionary<string, object?>
                    {
                        ["logic"] = group.Logic
                    }
                )
            };
        }

        // Resolves one group node into either a nested group expression or one criterion expression.
        private static Expression<Func<TEntity, bool>> BuildGroupNodeExpression<TEntity>(
            FilterGroupNode node,
            IReadOnlyList<Expression<Func<TEntity, bool>>> criterionExpressions
        ) where TEntity : class
        {
            var hasCriterionIndex = node.CriterionIndex.HasValue;
            var hasGroup = node.Group is not null;

            if (hasCriterionIndex == hasGroup)
            {
                throw new ValidationException(
                    FilterErrorCodes.InvalidGroup,
                    "Each filter group node must define either a criterion index or a nested group."
                );
            }

            if (node.Group is not null)
            {
                return BuildGroupExpression(node.Group, criterionExpressions);
            }

            var criterionIndex = node.CriterionIndex!.Value;
            if (criterionIndex < 0 || criterionIndex >= criterionExpressions.Count)
            {
                throw new ValidationException(
                    FilterErrorCodes.InvalidGroup,
                    $"Filter group references invalid criterion index '{criterionIndex}'.",
                    new Dictionary<string, object?>
                    {
                        ["criterionIndex"] = criterionIndex
                    }
                );
            }

            return criterionExpressions[criterionIndex];
        }

        // Applies logical NOT to exactly one child expression.
        private static Expression<Func<TEntity, bool>> BuildNotExpression<TEntity>(
            IReadOnlyList<Expression<Func<TEntity, bool>>> children
        ) where TEntity : class
        {
            if (children.Count != 1)
            {
                throw new ValidationException(
                    FilterErrorCodes.InvalidGroup,
                    "NOT filter groups must contain exactly one child."
                );
            }

            var parameter = Expression.Parameter(typeof(TEntity), "entity");
            var body = ReplaceParameter(children[0].Parameters[0], parameter, children[0].Body);

            return Expression.Lambda<Func<TEntity, bool>>(Expression.Not(body), parameter);
        }

        // Converts one filter criterion into a predicate against the mapped entity field.
        private static Expression<Func<TEntity, bool>> BuildCriterionExpression<TEntity>(
            FilterCriterion criterion,
            FilterQueryMap<TEntity> filterQueryMap
        ) where TEntity : class
        {
            if (!filterQueryMap.TryGetField(criterion.FieldName, out var mappedField))
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

            var values = (criterion.Values ?? [])
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim())
                .ToList();

            ValidateValueCount(criterion, values);

            var parameter = Expression.Parameter(typeof(TEntity), "entity");
            var selectorBody = ReplaceParameter(
                mappedField.Selector.Parameters[0],
                parameter,
                mappedField.Selector.Body
            );

            if (RequiresNoValue(criterion.Operator))
            {
                var noValueBody = BuildNoValueExpression(criterion, selectorBody);
                return Expression.Lambda<Func<TEntity, bool>>(noValueBody, parameter);
            }

            var valueExpressions = values
                .Select(value => BuildSingleValueExpression(
                    criterion,
                    selectorBody,
                    mappedField.FieldType,
                    value
                ))
                .ToList();

            var combinedBody = RequiresOrValueCombination(criterion.Operator)
                ? valueExpressions.Aggregate(Expression.OrElse)
                : valueExpressions.Aggregate(Expression.AndAlso);

            return Expression.Lambda<Func<TEntity, bool>>(combinedBody, parameter);
        }

        // Handles operators that do not expect any values from the payload.
        private static Expression BuildNoValueExpression(
            FilterCriterion criterion,
            Expression memberExpression
        )
        {
            return criterion.Operator switch
            {
                FilterOperator.IsNull => BuildIsNullExpression(criterion, memberExpression),
                _ => throw new ValidationException(
                    FilterErrorCodes.UnsupportedOperator,
                    $"Unsupported filter operator '{criterion.Operator}'.",
                    new Dictionary<string, object?>
                    {
                        ["fieldName"] = criterion.FieldName,
                        ["operator"] = criterion.Operator.ToString()
                    }
                )
            };
        }

        // Generates a null comparison and rejects fields that can never be null.
        private static Expression BuildIsNullExpression(
            FilterCriterion criterion,
            Expression memberExpression
        )
        {
            if (memberExpression.Type.IsValueType && Nullable.GetUnderlyingType(memberExpression.Type) is null)
            {
                throw new ValidationException(
                    FilterErrorCodes.UnsupportedOperator,
                    $"Operator '{criterion.Operator}' is only supported for nullable fields.",
                    new Dictionary<string, object?>
                    {
                        ["fieldName"] = criterion.FieldName,
                        ["operator"] = EnumUtils.ToDisplayValue(criterion.Operator)
                    }
                );
            }

            return Expression.Equal(memberExpression, Expression.Constant(null, memberExpression.Type));
        }

        // Builds one comparison for one value after parsing and optional temporal transformation.
        private static Expression BuildSingleValueExpression(
            FilterCriterion criterion,
            Expression memberExpression,
            Type fieldType,
            string rawValue
        )
        {
            var nullableFieldType = Nullable.GetUnderlyingType(fieldType);
            var targetFieldType = nullableFieldType ?? fieldType;
            var nullGuard = GetNullGuard(memberExpression);
            var comparableExpression = UnwrapNullable(memberExpression);
            var comparisonExpression = comparableExpression;
            object parsedValue;

            if (criterion.DateTimeFilterOptions is not null)
            {
                (comparisonExpression, parsedValue) = ApplyDateTimeFilterOptions(
                    comparableExpression,
                    targetFieldType,
                    rawValue,
                    criterion.DateTimeFilterOptions
                );
            }
            else
            {
                parsedValue = ParseFilterValue(rawValue, targetFieldType, criterion.FieldName);
            }

            var body = criterion.Operator switch
            {
                FilterOperator.Equal => Expression.Equal(
                    comparisonExpression,
                    Expression.Constant(parsedValue, comparisonExpression.Type)
                ),
                FilterOperator.GreaterThan => Expression.GreaterThan(
                    comparisonExpression,
                    Expression.Constant(parsedValue, comparisonExpression.Type)
                ),
                FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(
                    comparisonExpression,
                    Expression.Constant(parsedValue, comparisonExpression.Type)
                ),
                FilterOperator.LessThan => Expression.LessThan(
                    comparisonExpression,
                    Expression.Constant(parsedValue, comparisonExpression.Type)
                ),
                FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(
                    comparisonExpression,
                    Expression.Constant(parsedValue, comparisonExpression.Type)
                ),
                FilterOperator.Contains => BuildStringOperation(
                    comparisonExpression,
                    nameof(string.Contains),
                    parsedValue
                ),
                FilterOperator.StartsWith => BuildStringOperation(
                    comparisonExpression,
                    nameof(string.StartsWith),
                    parsedValue
                ),
                FilterOperator.EndsWith => BuildStringOperation(
                    comparisonExpression,
                    nameof(string.EndsWith),
                    parsedValue
                ),
                _ => throw new ValidationException(
                    FilterErrorCodes.UnsupportedOperator,
                    $"Unsupported filter operator '{criterion.Operator}'.",
                    new Dictionary<string, object?>
                    {
                        ["fieldName"] = criterion.FieldName,
                        ["operator"] = criterion.Operator.ToString()
                    }
                )
            };

            return nullGuard is null ? body : Expression.AndAlso(nullGuard, body);
        }

        // Applies date/month/year extraction when the request asks for temporal filtering.
        private static (Expression ComparisonExpression, object ParsedValue) ApplyDateTimeFilterOptions(
            Expression comparableExpression,
            Type fieldType,
            string rawValue,
            DateTimeFilterOptions options
        )
        {
            if (options.TemporalPartType == TemporalPartType.None)
            {
                return (
                    comparableExpression,
                    ParseFilterValue(rawValue, fieldType, null)
                );
            }

            if (fieldType != typeof(DateTimeOffset)
                && fieldType != typeof(DateTime)
                && fieldType != typeof(DateOnly))
            {
                throw new ValidationException(
                    FilterErrorCodes.UnsupportedOperator,
                    $"Temporal filters are not supported for type '{fieldType.Name}'."
                );
            }

            return options.TemporalPartType switch
            {
                TemporalPartType.Date => BuildDatePartComparison(
                    comparableExpression,
                    fieldType,
                    rawValue
                ),
                TemporalPartType.Month => (
                    Expression.Property(comparableExpression, nameof(DateTime.Month)),
                    int.Parse(rawValue, CultureInfo.InvariantCulture)
                ),
                TemporalPartType.Year => (
                    Expression.Property(comparableExpression, nameof(DateTime.Year)),
                    int.Parse(rawValue, CultureInfo.InvariantCulture)
                ),
                _ => throw new ValidationException(
                    FilterErrorCodes.UnsupportedOperator,
                    $"Unsupported temporal part '{options.TemporalPartType}'."
                )
            };
        }

        // Normalizes a date-only comparison for DateOnly and DateTime-like members.
        private static (Expression ComparisonExpression, object ParsedValue) BuildDatePartComparison(
            Expression comparableExpression,
            Type fieldType,
            string rawValue
        )
        {
            if (fieldType == typeof(DateOnly))
            {
                return (
                    comparableExpression,
                    DateOnly.Parse(rawValue, CultureInfo.InvariantCulture)
                );
            }

            return (
                Expression.Property(comparableExpression, nameof(DateTime.Date)),
                DateTime.Parse(rawValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind).Date
            );
        }

        // Translates string operators like contains and starts with into string method calls.
        private static Expression BuildStringOperation(
            Expression comparisonExpression,
            string methodName,
            object parsedValue
        )
        {
            if (comparisonExpression.Type != typeof(string))
            {
                throw new ValidationException(
                    FilterErrorCodes.UnsupportedOperator,
                    $"Operator '{methodName}' is only supported for string fields."
                );
            }

            var method = typeof(string).GetMethod(methodName, [typeof(string)])!;
            return Expression.Call(
                comparisonExpression,
                method,
                Expression.Constant((string)parsedValue, typeof(string))
            );
        }

        // Parses the raw string payload into the concrete CLR type expected by the mapped field.
        private static object ParseFilterValue(string rawValue, Type targetType, string? fieldName)
        {
            try
            {
                if (targetType == typeof(string))
                {
                    return rawValue;
                }

                if (targetType == typeof(Guid))
                {
                    return Guid.Parse(rawValue);
                }

                if (targetType == typeof(int))
                {
                    return int.Parse(rawValue, CultureInfo.InvariantCulture);
                }

                if (targetType == typeof(long))
                {
                    return long.Parse(rawValue, CultureInfo.InvariantCulture);
                }

                if (targetType == typeof(decimal))
                {
                    return decimal.Parse(rawValue, CultureInfo.InvariantCulture);
                }

                if (targetType == typeof(float))
                {
                    return float.Parse(rawValue, CultureInfo.InvariantCulture);
                }

                if (targetType == typeof(double))
                {
                    return double.Parse(rawValue, CultureInfo.InvariantCulture);
                }

                if (targetType == typeof(bool))
                {
                    return bool.Parse(rawValue);
                }

                if (targetType == typeof(DateTimeOffset))
                {
                    return DateTimeOffset.Parse(
                        rawValue,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.RoundtripKind
                    );
                }

                if (targetType == typeof(DateTime))
                {
                    return DateTime.Parse(
                        rawValue,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.RoundtripKind
                    );
                }

                if (targetType == typeof(DateOnly))
                {
                    return DateOnly.Parse(rawValue, CultureInfo.InvariantCulture);
                }

                if (targetType.IsEnum)
                {
                    return ParseEnumDisplayValue(targetType, rawValue);
                }
            }
            catch (Exception exception) when (exception is not ValidationException)
            {
                throw new ValidationException(
                    FilterErrorCodes.InvalidValue,
                    $"Invalid filter value '{rawValue}' for field '{fieldName}'.",
                    new Dictionary<string, object?>
                    {
                        ["fieldName"] = fieldName,
                        ["value"] = rawValue
                    }
                );
            }

            throw new ValidationException(
                FilterErrorCodes.InvalidValue,
                $"Unsupported filter value type '{targetType.Name}' for field '{fieldName}'.",
                new Dictionary<string, object?>
                {
                    ["fieldName"] = fieldName,
                    ["type"] = targetType.Name
                }
            );
        }

        // Reuses the enum display-value converter so FE-facing enum values can be filtered directly.
        private static object ParseEnumDisplayValue(Type enumType, string rawValue)
        {
            var method = typeof(EnumUtils)
                .GetMethod(nameof(EnumUtils.FromDisplayValue))!
                .MakeGenericMethod(enumType);

            try
            {
                return method.Invoke(null, [rawValue])!;
            }
            catch
            {
                throw new ValidationException(
                    FilterErrorCodes.InvalidValue,
                    $"Invalid filter value '{rawValue}' for enum '{enumType.Name}'.",
                    new Dictionary<string, object?>
                    {
                        ["type"] = enumType.Name,
                        ["value"] = rawValue
                    }
                );
            }
        }

        // Validates whether the operator received the correct number of values before building expressions.
        private static void ValidateValueCount(FilterCriterion criterion, IReadOnlyList<string> values)
        {
            if (RequiresNoValue(criterion.Operator))
            {
                if (values.Count != 0)
                {
                    throw new ValidationException(
                        FilterErrorCodes.InvalidValueCount,
                        $"Filter operator '{criterion.Operator}' does not accept values.",
                        new Dictionary<string, object?>
                        {
                            ["fieldName"] = criterion.FieldName,
                            ["operator"] = EnumUtils.ToDisplayValue(criterion.Operator),
                            ["valueCount"] = values.Count
                        }
                    );
                }

                return;
            }

            if (values.Count == 0)
            {
                throw new ValidationException(
                    FilterErrorCodes.InvalidValueCount,
                    $"Filter field '{criterion.FieldName}' must contain at least one value.",
                    new Dictionary<string, object?>
                    {
                        ["fieldName"] = criterion.FieldName
                    }
                );
            }

            if (!RequiresSingleValue(criterion.Operator))
            {
                return;
            }

            if (values.Count != 1)
            {
                throw new ValidationException(
                    FilterErrorCodes.InvalidValueCount,
                    $"Filter operator '{EnumUtils.ToDisplayValue(criterion.Operator)}' requires exactly one value.",
                    new Dictionary<string, object?>
                    {
                        ["fieldName"] = criterion.FieldName,
                        ["operator"] = EnumUtils.ToDisplayValue(criterion.Operator),
                        ["valueCount"] = values.Count
                    }
                );
            }
        }

        // Comparison operators like > and <= accept exactly one value.
        private static bool RequiresSingleValue(FilterOperator filterOperator)
        {
            return filterOperator is FilterOperator.GreaterThan
                or FilterOperator.GreaterThanOrEqual
                or FilterOperator.LessThan
                or FilterOperator.LessThanOrEqual;
        }

        // Null-check operators do not accept any payload values.
        private static bool RequiresNoValue(FilterOperator filterOperator)
        {
            return filterOperator is FilterOperator.IsNull;
        }

        // These operators combine multiple input values with OR inside one criterion.
        private static bool RequiresOrValueCombination(FilterOperator filterOperator)
        {
            return filterOperator is FilterOperator.Equal
                or FilterOperator.Contains
                or FilterOperator.StartsWith
                or FilterOperator.EndsWith;
        }

        // Rebinds parameters so separately-built lambdas can be merged into one expression tree.
        private static Expression ReplaceParameter(
            ParameterExpression source,
            Expression target,
            Expression expression
        )
        {
            return new ReplaceParameterVisitor(source, target).Visit(expression)!;
        }

        // Combines many predicates into one predicate using the provided boolean operator.
        private static Expression<Func<TEntity, bool>> CombineExpressions<TEntity>(
            IReadOnlyList<Expression<Func<TEntity, bool>>> expressions,
            Func<Expression, Expression, BinaryExpression> combiner
        ) where TEntity : class
        {
            if (expressions.Count == 0)
            {
                return _ => true;
            }

            var parameter = Expression.Parameter(typeof(TEntity), "entity");
            Expression? body = null;

            foreach (var expression in expressions)
            {
                var replacedBody = ReplaceParameter(expression.Parameters[0], parameter, expression.Body);
                body = body is null ? replacedBody : combiner(body, replacedBody);
            }

            return Expression.Lambda<Func<TEntity, bool>>(body!, parameter);
        }

        // Adds a null guard before member access when the field is nullable.
        private static Expression? GetNullGuard(Expression memberExpression)
        {
            if (!memberExpression.Type.IsValueType
                || Nullable.GetUnderlyingType(memberExpression.Type) is not null)
            {
                return Expression.NotEqual(memberExpression, Expression.Constant(null, memberExpression.Type));
            }

            return null;
        }

        // Converts Nullable<T> member access into its Value expression for typed comparisons.
        private static Expression UnwrapNullable(Expression expression)
        {
            return Nullable.GetUnderlyingType(expression.Type) is null
                ? expression
                : Expression.Property(expression, nameof(Nullable<int>.Value));
        }

        // Expression visitor used to replace one lambda parameter with another expression.
        private sealed class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression source;
            private readonly Expression target;

            public ReplaceParameterVisitor(ParameterExpression source, Expression target)
            {
                this.source = source;
                this.target = target;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == source ? target : base.VisitParameter(node);
            }
        }
    }

    private static class SortingExpression
    {
        // Applies all requested sorts in order using the field selectors from FilterQueryMap.
        public static IQueryable<TEntity> ApplySorting<TEntity>(
            IQueryable<TEntity> query,
            FilterQuery<TEntity> filterQuery
        ) where TEntity : class
        {
            var sortFields = filterQuery.SortFields;
            if (sortFields is null || sortFields.Count == 0)
            {
                return query;
            }

            var filterQueryMap = filterQuery.GetFilterQueryMap();
            IOrderedQueryable<TEntity>? orderedQuery = null;

            foreach (var sortField in sortFields)
            {
                if (!filterQueryMap.TryGetField(sortField.FieldName, out var mappedField))
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

                orderedQuery = ApplyOrdering(
                    orderedQuery ?? query,
                    mappedField.Selector,
                    sortField.IsAscending,
                    orderedQuery is null
                );
            }

            return orderedQuery ?? query;
        }

        // Chooses OrderBy/ThenBy and ascending/descending variants for one sort field.
        private static IOrderedQueryable<TEntity> ApplyOrdering<TEntity>(
            IQueryable<TEntity> query,
            LambdaExpression selector,
            bool isAscending,
            bool isFirstOrdering
        ) where TEntity : class
        {
            var methodName = isFirstOrdering
                ? (isAscending ? nameof(Queryable.OrderBy) : nameof(Queryable.OrderByDescending))
                : (isAscending ? nameof(Queryable.ThenBy) : nameof(Queryable.ThenByDescending));

            var orderingMethod = typeof(Queryable)
                .GetMethods()
                .Single(method =>
                    method.Name == methodName
                    && method.GetParameters().Length == 2
                )
                .MakeGenericMethod(typeof(TEntity), selector.ReturnType);

            return (IOrderedQueryable<TEntity>)orderingMethod.Invoke(null, [query, selector])!;
        }
    }

    private static class PaginationExpression
    {
        // Counts total rows, takes the requested page, and returns the final FilterResult.
        public static async Task<FilterResult<TResult>> ToFilterResultAsync<TEntity, TResult>(
            IQueryable<TEntity> query,
            FilterQuery<TEntity> filterQuery,
            Expression<Func<TEntity, TResult>> selector,
            CancellationToken cancellationToken
        )
            where TEntity : class
            where TResult : class
        {
            var totalItems = await query.CountAsync(cancellationToken);
            var currentPage = filterQuery.CurrentPage;
            var rowsPerPage = filterQuery.RowsPerPage;

            var items = await query
                .Skip((currentPage - 1) * rowsPerPage)
                .Take(rowsPerPage)
                .Select(selector)
                .ToListAsync(cancellationToken);

            return new FilterResult<TResult>
            {
                Items = items,
                CurrentPage = currentPage,
                RowsPerPage = rowsPerPage,
                TotalItems = totalItems,
                TotalPages = totalItems == 0
                    ? 0
                    : (int)Math.Ceiling(totalItems / (double)rowsPerPage)
            };
        }
    }
}
