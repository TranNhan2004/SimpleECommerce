using SimpleECommerceBackend.Application.Interfaces.Services.Translation;
using SimpleECommerceBackend.Application.Models.Translations;

namespace SimpleECommerceBackend.Infrastructure.Services.Translation;

public sealed class NullTranslationProvider : ITranslationProvider
{
    public string Name => "none";

    public Task<string> TranslateAsync(
        DynamicTranslationRequest request,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(request.SourceText);
    }
}
