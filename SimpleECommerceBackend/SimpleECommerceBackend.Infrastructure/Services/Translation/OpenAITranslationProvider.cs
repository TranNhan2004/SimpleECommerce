using Microsoft.Extensions.Options;
using OpenAI.Chat;
using SimpleECommerceBackend.Application.Interfaces.Services.Translation;
using SimpleECommerceBackend.Application.Models.Translations;
using SimpleECommerceBackend.Infrastructure.Options.Translation;

namespace SimpleECommerceBackend.Infrastructure.Services.Translation;

public partial class OpenAITranslationProvider : ITranslationProvider
{
    private readonly OpenAITranslationOptions _options;

    public OpenAITranslationProvider(IOptions<OpenAITranslationOptions> options)
    {
        _options = options.Value;
    }

    public string Name => "openai";

    public async Task<string> TranslateAsync(
        DynamicTranslationRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            throw new InvalidOperationException("Translation:OpenAI:ApiKey is not configured.");

        var client = new ChatClient(_options.Model, _options.ApiKey);
        var options = new ChatCompletionOptions();

        if (_options.MaxOutputTokenCount is > 0)
            options.MaxOutputTokenCount = _options.MaxOutputTokenCount;

        if (_options.Temperature is >= 0)
            options.Temperature = _options.Temperature;

        ChatMessage[] messages =
        [
            new SystemChatMessage(_options.Instructions),
            new UserChatMessage(CreatePrompt(request))
        ];

        var completion = await client.CompleteChatAsync(messages, options, cancellationToken);
        return completion.Value.Content[0].Text ?? request.SourceText;
    }

    private static string CreatePrompt(DynamicTranslationRequest request)
    {
        return
            $"Translate {request.EntityName}.{request.FieldName} from {request.SourceLocale} " +
            $"to {request.TargetLocale}.\nReturn plain text only.\n\nText:\n{request.SourceText}";
    }
}