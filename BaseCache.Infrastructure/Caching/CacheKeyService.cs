using BaseCache.Application.Common.Interfaces;

namespace BaseCache.Infrastructure.Caching;

public class CacheKeyService : ICacheKeyService
{
    public string GetCacheKey(string name, object id)
    {
        return $"{name}-{id}";
    }
}