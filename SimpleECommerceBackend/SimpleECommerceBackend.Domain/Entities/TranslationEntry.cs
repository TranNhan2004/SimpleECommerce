using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities;

public class TranslationEntry : Entity
{
    private TranslationEntry()
    {
        EntityName = string.Empty;
        FieldName = string.Empty;
        Locale = string.Empty;
        Value = string.Empty;
    }

    public TranslationEntry(
        Guid id,
        string entityName,
        string fieldName,
        Guid rowId,
        string locale,
        string value
    )
    {
        SetId(id);
        SetEntityName(entityName);
        SetFieldName(fieldName);
        SetRowId(rowId);
        SetLocale(locale);
        SetValue(value);
    }

    public string EntityName { get; private set; } = string.Empty;
    public string FieldName { get; private set; } = string.Empty;
    public Guid RowId { get; private set; }
    public string Locale { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;

    public void UpdateValue(string value)
    {
        SetValue(value);
    }

    private void SetEntityName(string entityName)
    {
        if (string.IsNullOrWhiteSpace(entityName))
        {
            throw new ValidationException(
                TranslationEntryErrorCode.EntityNameRequired,
                "Translation entity name is required.",
                new Dictionary<string, object?>
                {
                    ["field"] = "TranslationEntityName"
                }
            );
        }

        EntityName = entityName.Trim();
    }

    private void SetFieldName(string fieldName)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ValidationException(
                TranslationEntryErrorCode.FieldNameRequired,
                "Translation field name is required.",
                new Dictionary<string, object?>
                {
                    ["field"] = "TranslationFieldName"
                }
            );
        }

        FieldName = fieldName.Trim();
    }

    private void SetRowId(Guid rowId)
    {
        if (rowId == Guid.Empty)
        {
            throw new ValidationException(
                TranslationEntryErrorCode.RowIdRequired,
                "Translation row id is required.",
                new Dictionary<string, object?>
                {
                    ["field"] = "RowId"
                }
            );
        }

        RowId = rowId;
    }

    private void SetLocale(string locale)
    {
        if (string.IsNullOrWhiteSpace(locale))
        {
            throw new ValidationException(
                TranslationEntryErrorCode.LocaleRequired,
                "Translation locale is required.",
                new Dictionary<string, object?>
                {
                    ["field"] = "TranslationLocale"
                }
            );
        }

        Locale = locale.Trim().ToLowerInvariant();
    }

    private void SetValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ValidationException(
                TranslationEntryErrorCode.ValueRequired,
                "Translation value is required.",
                new Dictionary<string, object?>
                {
                    ["field"] = "TranslationValue"
                }
            );
        }

        Value = value.Trim();
    }
}
