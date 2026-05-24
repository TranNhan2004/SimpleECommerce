# FilterExtension

## Purpose

`FilterExtension` is the reusable infrastructure component that turns a `FilterQuery<TEntity>` into a real EF Core query pipeline.

It handles 3 concerns in one place:

- filtering
- sorting
- pagination

The main goal is to keep repositories and handlers small. A use case only needs:

1. an `IQueryable<TEntity>`
2. a `FilterQuery<TEntity>`
3. a `FilterQueryMap<TEntity>` that whitelists which FE field names are allowed

Then `FilterExtension` builds the final query and returns `FilterResult<T>`.

## Main Entry Point

File:
- [FilterExtension.cs](/home/thnhan1/SimpleECommerce/SimpleECommerceBackend/SimpleECommerceBackend.Infrastructure/Extensions/FilterExtension.cs)

Public methods:

- `ToFilterResultAsync<TEntity>(IQueryable<TEntity>, FilterQuery<TEntity>, CancellationToken)`
- `ToFilterResultAsync<TEntity, TResult>(IQueryable<TEntity>, FilterQuery<TEntity>, Expression<Func<TEntity, TResult>>, CancellationToken)`

The second overload is the real pipeline:

1. apply filtering
2. apply sorting
3. count total rows
4. apply `Skip` / `Take`
5. project to `TResult`
6. return `FilterResult<TResult>`

## Core Models

Relevant files:

- [FilterQuery.cs](/home/thnhan1/SimpleECommerce/SimpleECommerceBackend/SimpleECommerceBackend.Application/Models/Common/Filter/FilterQuery.cs)
- [FilterGroup.cs](/home/thnhan1/SimpleECommerce/SimpleECommerceBackend/SimpleECommerceBackend.Application/Models/Common/Filter/FilterGroup.cs)
- [FilterGroupNode.cs](/home/thnhan1/SimpleECommerce/SimpleECommerceBackend/SimpleECommerceBackend.Application/Models/Common/Filter/FilterGroupNode.cs)
- [FilterCriterion.cs](/home/thnhan1/SimpleECommerce/SimpleECommerceBackend/SimpleECommerceBackend.Application/Models/Common/Filter/FilterCriterion.cs)
- [FilterQueryMap.cs](/home/thnhan1/SimpleECommerce/SimpleECommerceBackend/SimpleECommerceBackend.Application/Models/Common/Filter/FilterQueryMap.cs)
- [FilterQueryMapField.cs](/home/thnhan1/SimpleECommerce/SimpleECommerceBackend/SimpleECommerceBackend.Application/Models/Common/Filter/FilterQueryMapField.cs)

### `FilterQuery<TEntity>`

This is the backend model consumed by the extension.

It contains:

- `FilterGroup? FilterGroup`
- `List<FilterCriterion>? FilterCriteria`
- `List<SortField>? SortFields`
- pagination values inherited from `PaginationQuery`

It also forces each query type to define:

- `GetFilterQueryMap()`

That map is the whitelist between FE field names and BE entity selectors.

### `FilterQueryMap<TEntity>`

This prevents arbitrary filtering on any entity property.

Example:

```csharp
public override FilterQueryMap<Category> GetFilterQueryMap()
{
    return new FilterQueryMap<Category>()
        .Map("name", category => category.Name)
        .Map("status", category => category.Status)
        .Map("createdAt", category => category.CreatedAt)
        .Map("updatedAt", category => category.UpdatedAt);
}
```

This means FE can filter using `name`, `status`, `createdAt`, `updatedAt`, but nothing else.

## Request Flow

The API request DTOs convert FE payloads into backend models.

Relevant files:

- [FilterRequest.cs](/home/thnhan1/SimpleECommerce/SimpleECommerceBackend/SimpleECommerceBackend.Api/Dtos/Common/Filter/FilterRequest.cs)
- [FilterCriterionRequest.cs](/home/thnhan1/SimpleECommerce/SimpleECommerceBackend/SimpleECommerceBackend.Api/Dtos/Common/Filter/FilterCriterionRequest.cs)
- [DateTimeFilterOptionsRequest.cs](/home/thnhan1/SimpleECommerce/SimpleECommerceBackend/SimpleECommerceBackend.Api/Dtos/Common/Filter/DateTimeFilterOptionsRequest.cs)
- [FilterGroupConverter.cs](/home/thnhan1/SimpleECommerce/SimpleECommerceBackend/SimpleECommerceBackend.Application/Utils/FilterGroupConverter.cs)

The API request is intentionally simpler than the backend tree model.

FE sends:

- `groupPattern`
- `filterCriteria`
- `sortFields`
- pagination values

`groupPattern` examples:

- `({0} AND {2}) OR {3}`
- `(NOT {1}) AND {2}`
- `(NOT ({1} AND {2}))`

The DTO layer converts FE display values into backend enums using `EnumUtils.FromDisplayValue<TEnum>()`, then `FilterGroupConverter` turns `groupPattern` into the backend `FilterGroup` tree.

Examples:

- `"and"` -> `FilterGroupLogic.And`
- `"contains"` -> `FilterOperator.Contains`
- `"date"` -> `TemporalPartType.Date`

## How Filtering Works

Inside `FilterExtension`, filtering is handled by the internal `FilterExpression` class.

### Step 1: Build one predicate per criterion

Each `FilterCriterion` becomes an `Expression<Func<TEntity, bool>>`.

The extension first:

1. finds the field in `FilterQueryMap`
2. validates the number of values for the operator
3. parses raw string values into the target CLR type
4. builds the comparison expression

If the field name is not registered in the map, it throws `ValidationException(FilterErrorCodes.UnknownField)`.

### Step 2: Normalize the filter group

Filtering is always glued together through `FilterGroup`.

`FilterGroup` is the backend tree model. FE does not send this tree directly anymore. Instead, FE sends `groupPattern`, and `FilterGroupConverter` builds the tree before `FilterExtension` executes the query.

If `groupPattern` is:

- `null`, or
- empty, or
- missing some criterion indexes

`FilterGroupConverter` builds or wraps it into a complete top-level `AND` group.

Example:

- criteria indexes: `0, 1, 2`
- input group: `OR(0, 1)`

Effective group becomes:

- `AND( OR(0, 1), 2 )`

This ensures all criteria are represented in one complete group tree before evaluation starts.

### Step 3: Convert the group tree into one expression

`BuildGroupExpression` walks the group recursively.

Supported group logic:

- `And`
- `Or`
- `Not`

Rules:

- `Not` must have exactly one child
- each `FilterGroupNode` must contain either `CriterionIndex` or nested `Group`
- referenced criterion indexes must be valid

## Operator Semantics

Supported operators come from [FilterOperator.cs](/home/thnhan1/SimpleECommerce/SimpleECommerceBackend/SimpleECommerceBackend.Domain/Enums/Common/FilterOperator.cs).

Current translation rules:

- `Equal`
- `GreaterThan`
- `GreaterThanOrEqual`
- `LessThan`
- `LessThanOrEqual`
- `Contains`
- `StartsWith`
- `EndsWith`
- `IsNull`

### Value count rules

- `IsNull` accepts no values
- `GreaterThan`, `GreaterThanOrEqual`, `LessThan`, `LessThanOrEqual` require exactly one value
- `Equal`, `Contains`, `StartsWith`, `EndsWith` can accept multiple values

When one criterion has multiple values for an OR-style operator, the extension combines them with `OR`.

Example:

```json
{
  "fieldName": "name",
  "operator": "contains",
  "values": ["phone", "laptop"]
}
```

becomes conceptually:

```csharp
entity => entity.Name.Contains("phone") || entity.Name.Contains("laptop")
```

## Type Parsing

Raw values arrive as strings from the request payload.

`ParseFilterValue` converts them into the mapped field type.

Currently supported target types:

- `string`
- `Guid`
- `int`
- `long`
- `decimal`
- `float`
- `double`
- `bool`
- `DateTimeOffset`
- `DateTime`
- `DateOnly`
- enums through `EnumUtils.FromDisplayValue<TEnum>()`

If parsing fails, the extension throws `ValidationException(FilterErrorCodes.InvalidValue)`.

## Date and Time Filtering

If a criterion includes `DateTimeFilterOptions`, the extension can reshape the comparison before applying the operator.

Current temporal parts:

- `None`
- `Date`
- `Month`
- `Year`

Examples:

- compare only the date portion of `CreatedAt`
- compare only the month portion
- compare only the year portion

Important note:

- `OffsetMinutes` exists in the request/model, but the current `FilterExtension` implementation does not apply timezone offset conversion yet.

## Null Handling

`IsNull` is supported explicitly.

Example:

```json
{
  "fieldName": "updatedAt",
  "operator": "is null",
  "values": []
}
```

The extension also adds null guards for nullable members before method calls or nullable value access.

This mainly protects expression building when:

- string methods are used
- nullable value types are unwrapped

## Sorting

Sorting is handled by the internal `SortingExpression` class.

Flow:

1. read `SortFields`
2. resolve each field through `FilterQueryMap`
3. apply `OrderBy` / `OrderByDescending`
4. apply `ThenBy` / `ThenByDescending` for the remaining fields

If a sort field is not registered in the map, the extension throws `ValidationException(FilterErrorCodes.UnknownField)`.

## Pagination

Pagination is handled by the internal `PaginationExpression` class.

Flow:

1. count total items after filtering and sorting
2. apply `Skip((CurrentPage - 1) * RowsPerPage)`
3. apply `Take(RowsPerPage)`
4. project to `TResult`
5. return `FilterResult<TResult>`

`FilterResult<TResult>` contains:

- `Items`
- `CurrentPage`
- `RowsPerPage`
- `TotalItems`
- `TotalPages`
- derived page metadata inherited from pagination result behavior

## Example End-to-End Shape

Conceptual payload:

```json
{
  "groupPattern": "({0} AND {1})",
  "filterCriteria": [
    {
      "fieldName": "name",
      "operator": "contains",
      "fieldType": "string",
      "values": ["phone", "laptop"]
    },
    {
      "fieldName": "updatedAt",
      "operator": "is null",
      "fieldType": "datetime",
      "values": []
    }
  ],
  "sortFields": [
    {
      "fieldName": "createdAt",
      "isAscending": false
    }
  ],
  "currentPage": 1,
  "rowsPerPage": 20
}
```

Conceptual execution:

1. resolve `name`, `updatedAt`, `createdAt` through `FilterQueryMap`
2. convert `groupPattern` into `FilterGroup`
3. build criterion expressions
4. combine them through the normalized `FilterGroup`
5. apply sorting
6. paginate
7. project and return `FilterResult<T>`

## Current Design Tradeoffs

What this design does well:

- keeps repositories smaller
- centralizes filter/sort/paging logic
- prevents FE from querying arbitrary fields
- supports nested boolean groups
- reuses enum display values consistently

Current limitations:

- `OffsetMinutes` is not yet used in temporal filtering
- the implementation is expression-heavy, so debugging can be harder than handwritten query code
- the filter map currently supports direct selector-based mapping; very custom business filters may still need special handling

## Recommended Usage

Use `FilterExtension` when:

- a list endpoint needs reusable filtering
- FE needs a shared payload structure for searching tables or admin lists
- the backend can expose a clear field whitelist through `FilterQueryMap`

Do not treat it as an unrestricted dynamic query engine. The intended design is:

- shared request shape
- backend-owned field map
- backend-controlled expression translation
