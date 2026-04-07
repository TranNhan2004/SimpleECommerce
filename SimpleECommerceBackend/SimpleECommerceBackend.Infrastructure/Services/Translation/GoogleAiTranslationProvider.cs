using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SimpleECommerceBackend.Application.Interfaces.Services.Translation;
using SimpleECommerceBackend.Application.Models.Translations;

namespace SimpleECommerceBackend.Infrastructure.Services.Translation;

[AutoConstructor]
public partial class GoogleAiTranslationProvider : ITranslationProvider
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<GoogleAiTranslationOptions> _options;

    public string Name => "googleai";

    public async Task<string> TranslateAsync(
        DynamicTranslationRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(_options.Value.ApiKey))
            throw new InvalidOperationException("Translation:GoogleAI:ApiKey is not configured.");

        var prompt =
            $"Translate {request.EntityName}.{request.FieldName} from {request.SourceLocale} to {request.TargetLocale}. Preserve product meaning and return plain text only.\n\nText:\n{request.SourceText}";

        using var message = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_options.Value.BaseUrl.TrimEnd('/')}/models/{_options.Value.Model}:generateContent?key={Uri.EscapeDataString(_options.Value.ApiKey)}"
        );
        message.Content = new StringContent(
            JsonSerializer.Serialize(new
            {
                contents = new object[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new
                            {
                                text = prompt
                            }
                        }
                    }
                }
            }),
            Encoding.UTF8,
            "application/json"
        );

        using var response = await _httpClient.SendAsync(message, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
        var root = document.RootElement;
        if (root.TryGetProperty("candidates", out var candidates)
            && candidates.ValueKind == JsonValueKind.Array
            && candidates.GetArrayLength() > 0)
        {
            var firstCandidate = candidates[0];
            if (firstCandidate.TryGetProperty("content", out var content)
                && content.TryGetProperty("parts", out var parts)
                && parts.ValueKind == JsonValueKind.Array
                && parts.GetArrayLength() > 0
                && parts[0].TryGetProperty("text", out var text))
                return text.GetString() ?? request.SourceText;
        }

        return request.SourceText;
    }
}