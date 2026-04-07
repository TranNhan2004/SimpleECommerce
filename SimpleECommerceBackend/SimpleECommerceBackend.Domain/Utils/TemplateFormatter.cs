namespace SimpleECommerceBackend.Domain.Utils;

public static class TemplateFormatter
{
    public static string Format(string template, IReadOnlyDictionary<string, object?>? parameters)
    {
        if (parameters == null || parameters.Count == 0)
            return template;

        var result = template;

        foreach (var kv in parameters)
        {
            var value = kv.Value?.ToString() ?? string.Empty;
            result = result.Replace($"{{{kv.Key}}}", value);
        }

        return result;
    }
}