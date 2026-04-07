using SimpleECommerceBackend.Application.Models.Translations;

namespace SimpleECommerceBackend.Application.Interfaces.Services.Translation;

public interface IDynamicTranslationService
{
    Task<string> TranslateAsync(
        DynamicTranslationRequest request,
        CancellationToken cancellationToken = default
    );
}
