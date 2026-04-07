using SimpleECommerceBackend.Application.Models.Translations;

namespace SimpleECommerceBackend.Application.Interfaces.Services.Translation;

public interface ITranslationProvider
{
    string Name { get; }

    Task<string> TranslateAsync(
        DynamicTranslationRequest request,
        CancellationToken cancellationToken = default
    );
}
