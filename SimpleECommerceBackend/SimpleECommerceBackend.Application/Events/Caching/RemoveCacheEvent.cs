using SimpleECommerceBackend.Application.Interfaces.Events;
using SimpleECommerceBackend.Application.Interfaces.Services.Caching;
using SimpleECommerceBackend.Application.Models.Events;

namespace SimpleECommerceBackend.Application.Events.Caching;

public class RemoveCacheEvent : IEventHandler<RemoveCacheEventModel>
{
    private readonly Serilog.ILogger _logger;
    private readonly ICacheService _cacheService;

    public RemoveCacheEvent(Serilog.ILogger logger, ICacheService cacheService)
    {
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task HandleAsync(RemoveCacheEventModel @event, CancellationToken cancellationToken = default)
    {
        if (@event.Keys != null && @event.Keys.Count > 0)
        {
            var keys = @event.Keys.ToHashSet().ToList();
            foreach (var key in keys)
            {
                await _cacheService.RemoveAsync(key, cancellationToken);
                _logger.Information("RemoveCacheEvent: Removed cache for key: {Key}", key);
            }
        }

        if (@event.PrefixKeys != null && @event.PrefixKeys.Count > 0)
        {
            var prefixKeys = @event.PrefixKeys.ToHashSet().ToList();
            foreach (var prefixKey in prefixKeys)
            {
                await _cacheService.RemoveByPrefixAsync(prefixKey, cancellationToken);
                _logger.Information("RemoveCacheEvent: Removed cache for prefix key: {PrefixKey}", prefixKey);
            }
        }
    }
}