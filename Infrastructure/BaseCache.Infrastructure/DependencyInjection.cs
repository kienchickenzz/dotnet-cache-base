/// <summary>
/// DependencyInjection registers all Infrastructure-layer services into the DI container.
///
/// <p>Centralizes service registration (caching, serialization) to keep Program.cs clean
/// and ensure Infrastructure concerns are configured in one place.</p>
/// </summary>

namespace BaseCache.Infrastructure;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

using BaseCache.Application.Common.ApplicationServices.Caching;
using BaseCache.Application.Common.ApplicationServices.Serializer;
using BaseCache.Infrastructure.Caching;
using BaseCache.Infrastructure.Serializer;
using BaseCache.Infrastructure.Settings;


/// <summary>
/// Extension methods for registering Infrastructure services into <see cref="IServiceCollection"/>.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Đăng ký toàn bộ service của tầng Infrastructure (caching, serializer).
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services
            ._AddCaching(config)
            ._AddSerializer();

        return services;
    }

    /// <summary>
    /// Đăng ký cache provider dựa trên cấu hình.
    /// Ưu tiên Redis nếu được cấu hình và kết nối thành công, ngược lại fallback về InMemory.
    /// </summary>
    private static IServiceCollection _AddCaching(this IServiceCollection services, IConfiguration config)
    {
        var settings = config.GetSection(nameof(CacheSettings)).Get<CacheSettings>()
            ?? new CacheSettings();
        services.AddSingleton(settings);

        services.AddScoped<ICacheKeyService, CacheKeyService>();

        var logger = services.BuildServiceProvider()
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(DependencyInjection));

        if (settings.Provider == CacheProvider.Redis)
        {
            if (_TryConnectRedis(settings.RedisConnection))
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = settings.RedisConnection;
                });
                services.AddTransient<ICacheService, RedisCacheService>();

                logger.LogInformation("Cache initialized: Redis");
                return services;
            }

            logger.LogWarning("Redis connection failed. Falling back to InMemory cache");
        }

        // Default hoặc Fallback: InMemory
        services.AddMemoryCache();
        services.AddTransient<ICacheService, InMemoryCacheService>();

        logger.LogInformation("Cache initialized: InMemory");
        return services;
    }

    /// <summary>
    /// Kiểm tra kết nối Redis có hoạt động không.
    /// </summary>
    private static bool _TryConnectRedis(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        try
        {
            using var connection = ConnectionMultiplexer.Connect(
                connectionString,
                options => options.ConnectTimeout = 5000);

            return connection.IsConnected;
        }
        catch
        {
            return false;
        }
    }


    /// <summary>
    /// Đăng ký serializer service.
    /// </summary>
    private static IServiceCollection _AddSerializer(this IServiceCollection services)
    {
        services.AddScoped<ISerializerService, NewtonSoftService>();
        return services;
    }
}
