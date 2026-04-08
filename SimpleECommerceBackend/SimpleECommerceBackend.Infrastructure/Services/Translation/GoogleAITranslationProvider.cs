using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Options;
using SimpleECommerceBackend.Application.Interfaces.Services.Translation;
using SimpleECommerceBackend.Application.Models.Translations;

namespace SimpleECommerceBackend.Infrastructure.Services.Translation;

[AutoConstructor]
public partial class GoogleAITranslationProvider : ITranslationProvider
{
    private readonly IOptions<GoogleAITranslationOptions> _options;

    public string Name => "googleai";

    public async Task<string> TranslateAsync(
        DynamicTranslationRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(_options.Value.ApiKey))
            throw new InvalidOperationException("Translation:GoogleAI:ApiKey is not configured.");

        await using var client = CreateClient();
        var response = await client.Models.GenerateContentAsync(
            model: _options.Value.Model,
            contents: CreatePrompt(request),
            config: CreateConfig(),
            cancellationToken: cancellationToken
        );

        return response.Text ?? request.SourceText;
    }

    private Client CreateClient()
    {
        return new Client(apiKey: _options.Value.ApiKey);
    }

    private GenerateContentConfig CreateConfig()
    {
        var config = new GenerateContentConfig();

        if (!string.IsNullOrWhiteSpace(_options.Value.SystemInstruction))
            config.SystemInstruction = new Content
            {
                Parts =
                [
                    new Part
                    {
                        Text = _options.Value.SystemInstruction
                    }
                ]
            };

        if (_options.Value.Temperature is not null)
            config.Temperature = _options.Value.Temperature;

        if (_options.Value.MaxOutputTokens is > 0)
            config.MaxOutputTokens = _options.Value.MaxOutputTokens;

        return config;
    }

    private static string CreatePrompt(DynamicTranslationRequest request)
    {
        return
            $"Translate {request.EntityName}.{request.FieldName} from {request.SourceLocale} to {request.TargetLocale}.\nReturn plain text only.\n\nText:\n{request.SourceText}";
    }
}
