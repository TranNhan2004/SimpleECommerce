namespace SimpleECommerceBackend.Infrastructure.Services.Translation;

public sealed class OpenAiTranslationOptions
{
    public const string SectionName = "Translation:OpenAI";

    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4.1-mini";
    public string BaseUrl { get; set; } = "https://api.openai.com/v1";
}
