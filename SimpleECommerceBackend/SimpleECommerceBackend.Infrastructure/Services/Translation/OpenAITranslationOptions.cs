namespace SimpleECommerceBackend.Infrastructure.Services.Translation;

public sealed class OpenAITranslationOptions
{
    public const string SectionName = "Translation:OpenAI";

    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4.1-mini";
    public int? MaxOutputTokenCount { get; set; } = 300;
    public string Instructions { get; set; } =
        "You are a translation engine. Return only the translated text. Preserve the original meaning and keep the response concise.";
}
