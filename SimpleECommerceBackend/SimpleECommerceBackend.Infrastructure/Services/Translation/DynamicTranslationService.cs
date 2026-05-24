using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Translation;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Application.Interfaces.Services.Translation;
using SimpleECommerceBackend.Application.Models.Translations;
using SimpleECommerceBackend.Domain.Entities.Translation;
using SimpleECommerceBackend.Domain.Utils;
using SimpleECommerceBackend.Infrastructure.Options.Translation;

namespace SimpleECommerceBackend.Infrastructure.Services.Translation;

public partial class DynamicTranslationService : IDynamicTranslationService
{
    private readonly ICacheService _cache;
    private readonly ILogger<DynamicTranslationService> _logger;
    private readonly IOptions<TranslationOptions> _options;
    private readonly IEnumerable<ITranslationProvider> _providers;
    private readonly ITranslationEntryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DynamicTranslationService(
        ICacheService cache,
        ILogger<DynamicTranslationService> logger,
        IOptions<TranslationOptions> options,
        IEnumerable<ITranslationProvider> providers,
        ITranslationEntryRepository repository,
        IUnitOfWork unitOfWork
    )
    {
        _cache = cache;
        _logger = logger;
        _options = options;
        _providers = providers;
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> TranslateAsync(
        DynamicTranslationRequest request,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(request.SourceText)) return request.SourceText;

        var normalizedSourceLocale = NormalizeLocale(request.SourceLocale);
        var normalizedTargetLocale = NormalizeLocale(request.TargetLocale);

        if (normalizedSourceLocale == normalizedTargetLocale) return request.SourceText;

        var cacheKey = BuildCacheKey(
            request.EntityName,
            request.FieldName,
            request.RowId,
            normalizedTargetLocale
        );

        var cachedValue = await _cache.GetStringAsync(cacheKey, cancellationToken);
        if (!string.IsNullOrWhiteSpace(cachedValue)) return cachedValue;

        var existing = await _repository.FindAsync(
            request.EntityName,
            request.FieldName,
            request.RowId,
            normalizedTargetLocale,
            trackChanges: false
        );

        if (existing is not null)
        {
            await _cache.SetStringAsync(cacheKey, existing.Value, GetCacheDuration(), cancellationToken);
            return existing.Value;
        }

        var provider = ResolveProvider();

        try
        {
            var translatedValue = await provider.TranslateAsync(
                request with
                {
                    SourceLocale = normalizedSourceLocale,
                    TargetLocale = normalizedTargetLocale,
                    SourceText = request.SourceText.Trim()
                },
                cancellationToken
            );

            if (string.IsNullOrWhiteSpace(translatedValue)) return request.SourceText;

            var entry = new TranslationEntry(
                UuidUtils.CreateV7(),
                request.EntityName,
                request.FieldName,
                request.RowId,
                normalizedTargetLocale,
                translatedValue
            );

            _repository.Add(entry);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _cache.SetStringAsync(cacheKey, translatedValue, GetCacheDuration(), cancellationToken);

            return translatedValue;
        }
        catch (Exception ex) when (_options.Value.UseSourceTextWhenProviderUnavailable)
        {
            _logger.LogWarning(
                ex,
                "Dynamic translation failed for {Entity}.{Field} row {RowId} to locale {Locale}. Returning source text.",
                request.EntityName,
                request.FieldName,
                request.RowId,
                normalizedTargetLocale
            );

            return request.SourceText;
        }
    }

    private ITranslationProvider ResolveProvider()
    {
        return _providers.FirstOrDefault(provider =>
                   string.Equals(provider.Name, _options.Value.Provider, StringComparison.OrdinalIgnoreCase))
               ?? _providers.First(provider =>
                   string.Equals(provider.Name, "none", StringComparison.OrdinalIgnoreCase));
    }

    private TimeSpan GetCacheDuration()
    {
        return TimeSpan.FromHours(Math.Max(_options.Value.CacheDurationHours, 0.25));
    }

    private static string BuildCacheKey(string entityName, string fieldName, Guid rowId, string locale)
    {
        return $"translation:{entityName}:{fieldName}:{rowId}:{locale}";
    }

    private static string NormalizeLocale(string locale)
    {
        return string.IsNullOrWhiteSpace(locale)
            ? "en"
            : locale.Trim().Split(',', ';')[0].ToLowerInvariant();
    }
}
