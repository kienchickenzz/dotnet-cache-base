namespace BaseCache.Infrastructure.Caching;

// TODO: Nên đặt trong Infra hay trong Application??
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
