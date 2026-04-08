using Microsoft.Extensions.Options;
using OpenAI.Chat;
using SimpleECommerceBackend.Application.Interfaces.Services.Translation;
using SimpleECommerceBackend.Application.Models.Translations;

namespace SimpleECommerceBackend.Infrastructure.Services.Translation;

[AutoConstructor]
public partial class OpenAITranslationProvider : ITranslationProvider
{
    private readonly IOptions<OpenAITranslationOptions> _options;

    public string Name => "openai";

    public async Task<string> TranslateAsync(
        DynamicTranslationRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(_options.Value.ApiKey))
            throw new InvalidOperationException("Translation:OpenAI:ApiKey is not configured.");

        var client = new ChatClient(_options.Value.Model, _options.Value.ApiKey);
        var options = new ChatCompletionOptions();

        if (_options.Value.MaxOutputTokenCount is > 0)
            options.MaxOutputTokenCount = _options.Value.MaxOutputTokenCount;

        ChatMessage[] messages = [
            new SystemChatMessage(_options.Value.Instructions),
            new UserChatMessage(CreatePrompt(request))
        ];

        var completion = await client.CompleteChatAsync(messages, options, cancellationToken);
        return completion.Value.Content[0].Text ?? request.SourceText;
    }

    private static string CreatePrompt(DynamicTranslationRequest request)
    {
        return
            $"Translate {request.EntityName}.{request.FieldName} from {request.SourceLocale} to {request.TargetLocale}.\nReturn plain text only.\n\nText:\n{request.SourceText}";
    }
}
