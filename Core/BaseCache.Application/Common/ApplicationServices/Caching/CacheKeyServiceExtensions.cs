namespace BaseCache.Application.Common.ApplicationServices.Caching;

using BaseCache.Domain.Common;


public static class CacheKeyServiceExtensions
{
    public static string GetCacheKey<TEntity>(this ICacheKeyService cacheKeyService, object id)
        where TEntity : BaseEntity =>
        cacheKeyService.GetCacheKey(typeof(TEntity).Name, id);
}