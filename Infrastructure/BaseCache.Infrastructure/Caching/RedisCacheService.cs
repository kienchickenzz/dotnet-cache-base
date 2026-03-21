namespace BaseCache.Infrastructure.Caching;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text;

using BaseCache.Application.Common.ApplicationServices.Caching;
using BaseCache.Application.Common.ApplicationServices.Serializer;


public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ISerializerService _serializer;
    private readonly CacheSettings _settings;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(
        IDistributedCache cache,
        ISerializerService serializer,
        CacheSettings settings,
        ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _serializer = serializer;
        _settings = settings;
        _logger = logger;
    }

    public T? Get<T>(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        try
        {
            var data = _cache.Get(key);
            return data is null ? default : Deserialize<T>(data);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis Get failed for key: {Key}", key);
            return default;
        }
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(key);

        try
        {
            var data = await _cache.GetAsync(key, token);
            return data is null ? default : Deserialize<T>(data);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis GetAsync failed for key: {Key}", key);
            return default;
        }
    }

    public void Refresh(string key)
    {
        try
        {
            _cache.Refresh(key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis Refresh failed for key: {Key}", key);
        }
    }

    public async Task RefreshAsync(string key, CancellationToken token = default)
    {
        try
        {
            await _cache.RefreshAsync(key, token);
            _logger.LogDebug("Redis Cache Refreshed: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis RefreshAsync failed for key: {Key}", key);
        }
    }

    public void Remove(string key)
    {
        try
        {
            _cache.Remove(key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis Remove failed for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken token = default)
    {
        try
        {
            await _cache.RemoveAsync(key, token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis RemoveAsync failed for key: {Key}", key);
        }
    }

    public void Set<T>(string key, T value, TimeSpan? slidingExpiration = null)
    {
        try
        {
            _cache.Set(key, Serialize(value), GetOptions(slidingExpiration));
            _logger.LogDebug("Added to Redis Cache: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis Set failed for key: {Key}", key);
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? slidingExpiration = null, CancellationToken token = default)
    {
        try
        {
            await _cache.SetAsync(key, Serialize(value), GetOptions(slidingExpiration), token);
            _logger.LogDebug("Added to Redis Cache: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis SetAsync failed for key: {Key}", key);
        }
    }

    private byte[] Serialize<T>(T item) =>
        Encoding.UTF8.GetBytes(_serializer.Serialize(item));

    private T Deserialize<T>(byte[] cachedData) =>
        _serializer.Deserialize<T>(Encoding.UTF8.GetString(cachedData));

    private DistributedCacheEntryOptions GetOptions(TimeSpan? slidingExpiration)
    {
        return new DistributedCacheEntryOptions
        {
            SlidingExpiration = slidingExpiration
                ?? TimeSpan.FromMinutes(_settings.DefaultSlidingExpirationMinutes)
        };
    }
}
