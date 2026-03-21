namespace BaseCache.Infrastructure;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

using BaseCache.Application.Common.ApplicationServices.Caching;
using BaseCache.Application.Common.ApplicationServices.Serializer;
using BaseCache.Infrastructure.Caching;
using BaseCache.Infrastructure.Serializer;


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
        var settings = config.GetSection(nameof(CacheSettings)).Get<CacheSettings>()
            ?? new CacheSettings();

        services.AddSingleton(settings);
        services.AddScoped<ICacheKeyService, CacheKeyService>();

        if (settings.Provider == CacheProvider.Redis && TryConnectRedis(settings))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = settings.RedisConnection;
            });
            services.AddTransient<ICacheService, RedisCacheService>();

            return services;
        }

        // Default hoặc Fallback: InMemory
        services.AddMemoryCache();
        services.AddTransient<ICacheService, InMemoryCacheService>();

        return services;
    }

    private static bool TryConnectRedis(CacheSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.RedisConnection))
        {
            return false;
        }

        try
        {
            using var connection = ConnectionMultiplexer.Connect(
                settings.RedisConnection,
                options => options.ConnectTimeout = 5000);

            return connection.IsConnected;
        }
        catch (Exception ex)
        {
            // Log warning - fallback to InMemory
            Console.WriteLine($"[Warning] Redis connection failed: {ex.Message}. Falling back to InMemory cache.");
            return false;
        }
    }


    private static IServiceCollection AddSerializer(this IServiceCollection services)
    {
        services.AddScoped<ISerializerService, NewtonSoftService>();
        return services;
    }
}
