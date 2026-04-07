using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SimpleECommerceBackend.Application.Interfaces.Services.Translation;
using SimpleECommerceBackend.Application.Models.Translations;

namespace SimpleECommerceBackend.Infrastructure.Services.Translation;

public sealed class OpenAiTranslationProvider : ITranslationProvider
{
    private readonly HttpClient _httpClient;
    private readonly OpenAiTranslationOptions _options;

    public OpenAiTranslationProvider(
        HttpClient httpClient,
        IOptions<OpenAiTranslationOptions> options
    )
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public string Name => "openai";

    public async Task<string> TranslateAsync(
        DynamicTranslationRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException("Translation:OpenAI:ApiKey is not configured.");
        }

        using var message = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_options.BaseUrl.TrimEnd('/')}/responses"
        );
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        message.Content = new StringContent(
            JsonSerializer.Serialize(new
            {
                model = _options.Model,
                input = new object[]
                {
                    new
                    {
                        role = "system",
                        content = new object[]
                        {
                            new
                            {
                                type = "input_text",
                                text = "You are a translation engine. Return only the translated text."
                            }
                        }
                    },
                    new
                    {
                        role = "user",
                        content = new object[]
                        {
                            new
                            {
                                type = "input_text",
                                text =
                                    $"Translate {request.EntityName}.{request.FieldName} from {request.SourceLocale} to {request.TargetLocale}. Preserve product meaning and return plain text only.\n\nText:\n{request.SourceText}"
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
        if (document.RootElement.TryGetProperty("output_text", out var outputText))
        {
            return outputText.GetString() ?? request.SourceText;
        }

        return request.SourceText;
    }
}
