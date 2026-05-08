using SimpleECommerceBackend.Domain.Constants.ErrorCodes;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Domain.Entities.Translation;

public class TranslationEntry : Entity
{
    public TranslationEntry()
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
        Id = id;
        EntityName = entityName;
        FieldName = fieldName;
        RowId = rowId;
        Locale = locale;
        Value = value;
    }

    private string _entityName = string.Empty;
    private string _fieldName = string.Empty;
    private Guid _rowId;
    private string _locale = string.Empty;
    private string _value = string.Empty;

    public string EntityName
    {
        get => _entityName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    TranslationEntryErrorCodes.EntityNameRequired,
                    "Translation entity name is required.",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "TranslationEntityName"
                    }
                );

            _entityName = value.Trim();
        }
    }

    public string FieldName
    {
        get => _fieldName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    TranslationEntryErrorCodes.FieldNameRequired,
                    "Translation field name is required.",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "TranslationFieldName"
                    }
                );

            _fieldName = value.Trim();
        }
    }

    public Guid RowId
    {
        get => _rowId;
        set
        {
            if (value == Guid.Empty)
                throw new ValidationException(
                    TranslationEntryErrorCodes.RowIdRequired,
                    "Translation row id is required.",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "RowId"
                    }
                );

            _rowId = value;
        }
    }

    public string Locale
    {
        get => _locale;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    TranslationEntryErrorCodes.LocaleRequired,
                    "Translation locale is required.",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "TranslationLocale"
                    }
                );

            _locale = value.Trim().ToLowerInvariant();
        }
    }

    public string Value
    {
        get => _value;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ValidationException(
                    TranslationEntryErrorCodes.ValueRequired,
                    "Translation value is required.",
                    new Dictionary<string, object?>
                    {
                        ["field"] = "TranslationValue"
                    }
                );

            _value = value.Trim();
        }
    }

    public void UpdateValue(string value)
    {
        Value = value;
    }
}
