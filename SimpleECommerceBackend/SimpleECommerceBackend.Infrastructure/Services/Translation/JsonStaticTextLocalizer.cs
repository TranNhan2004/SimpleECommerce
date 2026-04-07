using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SimpleECommerceBackend.Application.Interfaces.Services.Translation;
using SimpleECommerceBackend.Application.Models.Translations;

namespace SimpleECommerceBackend.Infrastructure.Services.Translation;

public sealed class JsonStaticTextLocalizer : IStaticTextLocalizer
{
    private readonly string _contentRootPath;
    private readonly TranslationOptions _options;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    private readonly Lock _sync = new();
    private Dictionary<string, TranslationResource> _resources = new(StringComparer.OrdinalIgnoreCase);

    public JsonStaticTextLocalizer(
        IHostEnvironment hostEnvironment,
        IOptions<TranslationOptions> options
    )
    {
        _contentRootPath = hostEnvironment.ContentRootPath;
        _options = options.Value;
    }

    public string LocalizeProblemTitle(string key, string locale)
    {
        var resource = GetResource(locale);
        if (resource.ProblemTitles.TryGetValue(key, out var title))
        {
            return title;
        }

        return GetFallbackResource().ProblemTitles.TryGetValue(key, out var fallbackTitle)
            ? fallbackTitle
            : key;
    }

    public LocalizedErrorMessage LocalizeError(
        string errorCode,
        IReadOnlyDictionary<string, object?>? details,
        string locale
    )
    {
        var resource = GetResource(locale);
        var fieldKey = GetFieldKey(details);
        var fieldDisplayName = fieldKey is null
            ? null
            : LocalizeFieldName(fieldKey, locale);

        var exactTemplate = GetTemplate(resource.ErrorTemplates, errorCode)
                            ?? GetTemplate(GetFallbackResource().ErrorTemplates, errorCode);
        if (!string.IsNullOrWhiteSpace(exactTemplate))
        {
            var values = CreateTemplateValues(details, fieldKey, fieldDisplayName);
            return new LocalizedErrorMessage(Format(exactTemplate, values), fieldKey, fieldDisplayName);
        }

        return new LocalizedErrorMessage(errorCode, fieldKey, fieldDisplayName);
    }

    public string LocalizeFieldName(string fieldName, string locale)
    {
        var resource = GetResource(locale);
        if (resource.Fields.TryGetValue(fieldName, out var localizedField))
        {
            return localizedField;
        }

        var normalizedFieldName = NormalizeFieldLookupKey(fieldName);

        var localizedByAlias = resource.Fields.FirstOrDefault(pair =>
            NormalizeFieldLookupKey(pair.Key) == normalizedFieldName);
        if (!string.IsNullOrWhiteSpace(localizedByAlias.Value))
        {
            return localizedByAlias.Value;
        }

        var fallbackResource = GetFallbackResource();
        if (fallbackResource.Fields.TryGetValue(fieldName, out var fallbackField))
        {
            return fallbackField;
        }

        var fallbackByAlias = fallbackResource.Fields.FirstOrDefault(pair =>
            NormalizeFieldLookupKey(pair.Key) == normalizedFieldName);
        return !string.IsNullOrWhiteSpace(fallbackByAlias.Value)
            ? fallbackByAlias.Value
            : Humanize(fieldName);
    }

    private TranslationResource GetResource(string locale)
    {
        var normalizedLocale = NormalizeLocale(locale);

        lock (_sync)
        {
            if (_resources.TryGetValue(normalizedLocale, out var resource))
            {
                return resource;
            }

            resource = LoadResource(normalizedLocale);
            _resources[normalizedLocale] = resource;
            return resource;
        }
    }

    private TranslationResource GetFallbackResource()
    {
        return GetResource(_options.DefaultLocale);
    }

    private TranslationResource LoadResource(string locale)
    {
        var path = Path.Combine(_contentRootPath, _options.StaticResourcesPath, $"errormessages.{locale}.json");
        if (!File.Exists(path))
        {
            return new TranslationResource();
        }

        var document = JsonSerializer.Deserialize<TranslationResourceDocument>(
            File.ReadAllText(path),
            _serializerOptions
        );

        return TranslationResource.FromDocument(document);
    }

    private static string? GetFieldKey(IReadOnlyDictionary<string, object?>? details)
    {
        if (details is null)
        {
            return null;
        }

        if (details.TryGetValue("field", out var fieldValue) && fieldValue is not null)
        {
            return GetFieldKey(fieldValue.ToString());
        }

        return null;
    }

    private static string? GetFieldKey(string? rawField)
    {
        if (string.IsNullOrWhiteSpace(rawField))
        {
            return null;
        }

        return rawField.Trim()
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("_", string.Empty, StringComparison.Ordinal);
    }

    private static Dictionary<string, string> CreateTemplateValues(
        IReadOnlyDictionary<string, object?>? details,
        string? fieldKey,
        string? fieldDisplayName
    )
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (details is not null)
        {
            foreach (var pair in details)
            {
                values[pair.Key] = pair.Value?.ToString() ?? string.Empty;
            }
        }

        if (!string.IsNullOrWhiteSpace(fieldKey))
        {
            values["fieldKey"] = fieldKey;
        }

        if (!string.IsNullOrWhiteSpace(fieldDisplayName))
        {
            values["field"] = fieldDisplayName;
            values["fieldDisplayName"] = fieldDisplayName;
        }

        return values;
    }

    private static string? GetTemplate(Dictionary<string, string> templates, string errorCode)
    {
        return templates.TryGetValue(errorCode, out var template) ? template : null;
    }

    private static string Format(string template, IReadOnlyDictionary<string, string> values)
    {
        var result = template;
        foreach (var pair in values)
        {
            result = result.Replace($"{{{pair.Key}}}", pair.Value, StringComparison.OrdinalIgnoreCase);
        }

        return result;
    }

    private static string NormalizeLocale(string locale)
    {
        var normalized = string.IsNullOrWhiteSpace(locale)
            ? "en"
            : locale.Trim().Split(',', ';')[0].ToLowerInvariant();

        var separatorIndex = normalized.IndexOf('-');
        return separatorIndex >= 0 ? normalized[..separatorIndex] : normalized;
    }

    private static string Humanize(string value)
    {
        var words = Regex.Replace(value, "(\\B[A-Z])", " $1");
        return words.Replace('-', ' ').Trim();
    }

    private static string NormalizeFieldLookupKey(string value)
    {
        return Regex.Replace(value, "[^a-z0-9]", string.Empty, RegexOptions.IgnoreCase)
            .ToLowerInvariant();
    }

    private sealed class TranslationResource
    {
        public Dictionary<string, string> ProblemTitles { get; init; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> Fields { get; init; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, string> ErrorTemplates { get; init; } = new(StringComparer.OrdinalIgnoreCase);

        public static TranslationResource FromDocument(TranslationResourceDocument? document)
        {
            if (document is null)
            {
                return new TranslationResource();
            }

            return new TranslationResource
            {
                ProblemTitles = new Dictionary<string, string>(
                    document.ProblemTitles ?? new Dictionary<string, string>(),
                    StringComparer.OrdinalIgnoreCase
                ),
                Fields = new Dictionary<string, string>(
                    document.Fields ?? new Dictionary<string, string>(),
                    StringComparer.OrdinalIgnoreCase
                ),
                ErrorTemplates = new Dictionary<string, string>(
                    document.Errors ?? new Dictionary<string, string>(),
                    StringComparer.OrdinalIgnoreCase
                )
            };
        }
    }

    private sealed class TranslationResourceDocument
    {
        public Dictionary<string, string>? ProblemTitles { get; init; }
        public Dictionary<string, string>? Fields { get; init; }
        public Dictionary<string, string>? Errors { get; init; }
    }
}
