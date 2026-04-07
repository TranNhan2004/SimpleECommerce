# Translation Design

## Goal

Centralize translation into one module with two paths:

1. Static translation for API error messages and field display names.
2. Dynamic translation for mutable content such as product name and description.

## Structure

### Schemas

- `business`: default schema for core business entities.
- `translation`: isolated schema for translation persistence.

### Translation table

`translation.Translations`

- `Id`
- `EntityName`
- `FieldName`
- `RowId`
- `Locale`
- `Value`

Unique key:

- `(EntityName, FieldName, RowId, Locale)`

## Runtime flow

### Static translation

1. Domain throws an `ExceptionBase` with an `ErrorCode` and optional `Details`.
2. `GlobalExceptionHandlerMiddleware` resolves locale from `Accept-Language`.
3. `IStaticTextLocalizer` loads JSON resources from `Api/ErrorMessages`.
4. Response contains localized `Title`, `Detail`, `Errors`, `errorCode`, and localized field display name.

### Dynamic translation

1. Call `IDynamicTranslationService.TranslateAsync`.
2. Check Redis/distributed cache first.
3. If miss, query `translation.Translations` with Dapper.
4. If miss, call configured LLM provider (`OpenAI` or `GoogleAI`).
5. Persist result to `translation.Translations`.
6. Write result back to cache.

## What is ok

- Static and dynamic translation are separated cleanly.
- Translation persistence is isolated from business tables.
- Dynamic translation lookup is efficient because it uses cache before DB before LLM.
- Dapper is used only for translation reads/writes, so business repositories can stay on EF Core.
- API now returns a stable `errorCode` plus localized text, which is better for UI handling.

## What is not ok yet

### 1. Existing domain errors are still mostly raw English strings

Problem:

- A lot of current `ValidationException` calls still use text as the `ErrorCode`.
- Static localization works now through pattern matching, but that is a compatibility layer, not a strong contract.

Fix:

- Replace raw messages with stable error keys such as `Product_Name_Required`.
- Put variable values into `Details`, for example `field`, `max`, `value`.

### 2. Translation rows can become stale when source content changes

Problem:

- Current table shape does not know whether the source text changed after translation was generated.

Fix:

- Add `SourceHash` or `SourceUpdatedAt` to the translation table.
- Or explicitly delete translation rows whenever the source field changes.

### 3. Moving existing tables to `business` schema is a breaking database migration

Problem:

- Setting `business` as the default schema is correct architecturally, but existing deployments on `dbo` need a real migration plan.

Fix:

- Create an explicit migration that moves current tables from `dbo` to `business`.
- Run that migration in a controlled release with backup and rollback steps.

### 4. LLM translation quality and cost are not controlled enough by table design alone

Problem:

- Same content can be sent repeatedly if cache is cold and DB is cleared.
- Some fields should never be translated automatically.

Fix:

- Add a whitelist for translatable entity fields.
- Add rate limits, retry policy, and provider timeout.
- Optionally deduplicate by source hash across rows for identical content.

### 5. `Value` only stores final translated text

Problem:

- There is no audit trail of which provider/model produced the text.

Fix:

- Add optional metadata columns later: `Provider`, `Model`, `CreatedAt`, `UpdatedAt`, `SourceHash`.

## Recommended next step

Start migrating domain exceptions to stable error keys first. That is the highest leverage improvement because it makes UI translation deterministic and removes regex-based fallback logic over time.
