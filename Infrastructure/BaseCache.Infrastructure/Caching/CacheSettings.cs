namespace BaseCache.Infrastructure.Caching;


public enum CacheProvider
{
    InMemory,
    Redis
}

public class CacheSettings
{
    public CacheProvider Provider { get; set; } = CacheProvider.InMemory;
    public string? RedisConnection { get; set; }
    public int DefaultSlidingExpirationMinutes { get; set; } = 10;
}
