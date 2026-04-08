namespace SimpleECommerceBackend.Application.Models.Translations;

public sealed record DynamicTranslationRequest(
    string EntityName,
    string FieldName,
    Guid RowId,
    string SourceLocale,
    string TargetLocale,
    string SourceText
);
