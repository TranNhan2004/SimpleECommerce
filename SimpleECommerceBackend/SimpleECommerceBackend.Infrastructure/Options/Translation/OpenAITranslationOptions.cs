namespace SimpleECommerceBackend.Infrastructure.Options.Translation;

public sealed class OpenAITranslationOptions
{
    public const string SectionName = "TranslationOptions:OpenAI";

    public string ApiKey { get; init; } = null!;
    public string Model { get; init; } = null!;
    public float? Temperature { get; init; }
    public int? MaxOutputTokenCount { get; init; }
    public string Instructions { get; init; } = null!;
}