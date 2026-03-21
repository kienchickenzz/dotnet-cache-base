namespace BaseCache.Infrastructure.Caching;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using BaseCache.Application.Common.ApplicationServices.Caching;


public class InMemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly CacheSettings _settings;
    private readonly ILogger<InMemoryCacheService> _logger;

    public InMemoryCacheService(
        IMemoryCache cache,
        CacheSettings settings,
        ILogger<InMemoryCacheService> logger)
    {
        _cache = cache;
        _settings = settings;
        _logger = logger;
    }

    public T? Get<T>(string key) =>
        _cache.Get<T>(key);

    public Task<T?> GetAsync<T>(string key, CancellationToken token = default) =>
        Task.FromResult(Get<T>(key));

    public void Refresh(string key) =>
        _cache.TryGetValue(key, out _);

    public Task RefreshAsync(string key, CancellationToken token = default)
    {
        Refresh(key);
        return Task.CompletedTask;
    }

    public void Remove(string key) =>
        _cache.Remove(key);

    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        Remove(key);
        return Task.CompletedTask;
    }

    public void Set<T>(string key, T value, TimeSpan? slidingExpiration = null)
    {
        slidingExpiration ??= TimeSpan.FromMinutes(_settings.DefaultSlidingExpirationMinutes);

        _cache.Set(key, value, new MemoryCacheEntryOptions
        {
            SlidingExpiration = slidingExpiration
        });

        _logger.LogDebug("Added to InMemory Cache: {Key}", key);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, CancellationToken token = default)
    {
        Set(key, value, slidingExpiration);
        return Task.CompletedTask;
    }
}
