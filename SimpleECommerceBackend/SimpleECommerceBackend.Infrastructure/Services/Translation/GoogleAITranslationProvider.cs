using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Options;
using SimpleECommerceBackend.Application.Interfaces.Services.Translation;
using SimpleECommerceBackend.Application.Models.Translations;
using SimpleECommerceBackend.Infrastructure.Options.Translation;

namespace SimpleECommerceBackend.Infrastructure.Services.Translation;

public class GoogleAITranslationProvider : ITranslationProvider
{
    private readonly GoogleAITranslationOptions _options;

    public GoogleAITranslationProvider(IOptions<GoogleAITranslationOptions> options)
    {
        _options = options.Value;
    }

    public string Name => "googleai";

    public async Task<string> TranslateAsync(
        DynamicTranslationRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            throw new InvalidOperationException("TranslationOptions:GoogleAI:ApiKey is not configured.");

        await using var client = CreateClient();
        var response = await client.Models.GenerateContentAsync(
            _options.Model,
            CreatePrompt(request),
            CreateConfig(),
            cancellationToken
        );

        return response.Text ?? request.SourceText;
    }

    private Client CreateClient()
    {
        return new Client(apiKey: _options.ApiKey);
    }

    private GenerateContentConfig CreateConfig()
    {
        var config = new GenerateContentConfig();

        if (!string.IsNullOrWhiteSpace(_options.SystemInstruction))
            config.SystemInstruction = new Content
            {
                Parts =
                [
                    new Part
                    {
                        Text = _options.SystemInstruction
                    }
                ]
            };

        if (_options.Temperature is >= 0)
            config.Temperature = _options.Temperature;

        if (_options.MaxOutputTokens is > 0)
            config.MaxOutputTokens = _options.MaxOutputTokens;

        return config;
    }

    private static string CreatePrompt(DynamicTranslationRequest request)
    {
        return
            $"Translate {request.EntityName}.{request.FieldName} from {request.SourceLocale} to {request.TargetLocale}.\nReturn plain text only.\n\nText:\n{request.SourceText}";
    }
}