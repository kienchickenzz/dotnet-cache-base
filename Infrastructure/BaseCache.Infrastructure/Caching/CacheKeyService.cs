namespace BaseCache.Infrastructure.Caching;

using BaseCache.Application.Common.ApplicationServices.Caching;


public class CacheKeyService : ICacheKeyService
{
    public string GetCacheKey(string name, object id)
    {
        return $"{name}-{id}";
    }
}