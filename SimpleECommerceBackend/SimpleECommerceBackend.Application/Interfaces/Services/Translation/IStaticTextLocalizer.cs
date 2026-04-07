using SimpleECommerceBackend.Application.Models.Translations;

namespace SimpleECommerceBackend.Application.Interfaces.Services.Translation;

public interface IStaticTextLocalizer
{
    string LocalizeProblemTitle(string key, string locale);
    LocalizedErrorMessage LocalizeError(string errorCode, IReadOnlyDictionary<string, object?>? details, string locale);
    string LocalizeFieldName(string fieldName, string locale);
}
