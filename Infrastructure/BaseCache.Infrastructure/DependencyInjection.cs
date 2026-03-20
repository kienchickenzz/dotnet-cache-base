namespace BaseCache.Infrastructure;

using BaseCache.Application.Common.Interfaces;
using BaseCache.Infrastructure.Caching;
using BaseCache.Infrastructure.Serializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddCaching(config)
            .AddSerializer();

        return services;
    }

    private static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<ICacheKeyService, CacheKeyService>();

        var settings = config.GetSection(nameof(CacheSettings)).Get<CacheSettings>();
        if (settings is null) return services;

        if (settings.UseDistributedCache)
        {
            if (settings.PreferRedis)
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = settings.RedisURL;
                    options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions()
                    {
                        AbortOnConnectFail = true,
                        EndPoints = { settings.RedisURL! }
                    };
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            services.AddTransient<ICacheService, DistributedCacheService>();
        }
        else
        {
            services.AddTransient<ICacheService, LocalCacheService>();
        }

        // TODO: Dù dùng Redis hay không thì vẫn có cache trong memory là sao?? 
        services.AddMemoryCache();
        return services;
    }

    private static IServiceCollection AddSerializer(this IServiceCollection services)
    {
        services.AddScoped<ISerializerService, NewtonSoftService>();
        return services;
    }
}
