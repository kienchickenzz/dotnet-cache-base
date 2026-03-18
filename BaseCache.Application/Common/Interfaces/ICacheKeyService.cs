namespace BaseCache.Application.Common.Interfaces;

public interface ICacheKeyService
{
    public string GetCacheKey(string name, object id);
}