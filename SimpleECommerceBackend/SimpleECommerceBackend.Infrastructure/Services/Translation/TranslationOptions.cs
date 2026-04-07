namespace SimpleECommerceBackend.Infrastructure.Services.Translation;

public sealed class TranslationOptions
{
    public const string SectionName = "Translation";

    public string DefaultLocale { get; set; } = "en";
    public string StaticResourcesPath { get; set; } = "ErrorMessages";
    public string Provider { get; set; } = "none";
    public double CacheDurationHours { get; set; } = 24;
    public bool UseSourceTextWhenProviderUnavailable { get; set; } = true;
    public List<string> SupportedLocales { get; set; } = ["en"];
}
