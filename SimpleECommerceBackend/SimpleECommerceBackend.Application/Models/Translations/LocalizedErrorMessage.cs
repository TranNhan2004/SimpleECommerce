namespace SimpleECommerceBackend.Application.Models.Translations;

public sealed record LocalizedErrorMessage(
    string Message,
    string? FieldKey = null,
    string? FieldDisplayName = null
);
