namespace SimpleECommerceBackend.Infrastructure.Services.Translation;

public sealed class GoogleAITranslationOptions
{
    public const string SectionName = "Translation:GoogleAI";

    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gemini-2.0-flash";
    public float? Temperature { get; set; } = 0.2f;
    public int? MaxOutputTokens { get; set; } = 300;
    public string SystemInstruction { get; set; } =
        "You are a translation engine. Return only the translated text. Preserve the original meaning and keep the response concise.";
}
