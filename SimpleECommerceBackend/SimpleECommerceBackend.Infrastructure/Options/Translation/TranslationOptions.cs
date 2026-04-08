namespace SimpleECommerceBackend.Infrastructure.Options.Translation;

public sealed class TranslationOptions
{
    public const string SectionName = "TranslationOptions";

    public string DefaultLocale { get; init; } = null!;
    public string StaticResourcesPath { get; init; } = null!;
    public string Provider { get; init; } = null!;
    public double CacheDurationHours { get; init; }
    public bool UseSourceTextWhenProviderUnavailable { get; init; }
    public List<string> SupportedLocales { get; init; } = null!;
}