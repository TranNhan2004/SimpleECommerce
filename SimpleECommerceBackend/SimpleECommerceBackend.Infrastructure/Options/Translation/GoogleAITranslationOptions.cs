namespace SimpleECommerceBackend.Infrastructure.Options.Translation;

public sealed class GoogleAITranslationOptions
{
    public const string SectionName = "TranslationOptions:GoogleAI";

    public string ApiKey { get; init; } = null!;
    public string Model { get; init; } = null!;
    public double? Temperature { get; init; }
    public int? MaxOutputTokens { get; init; }
    public string SystemInstruction { get; init; } = null!;
}