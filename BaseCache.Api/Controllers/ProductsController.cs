using BaseCache.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BaseCache.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ICacheService _cache;
    private readonly ICacheKeyService _cacheKey;
    private readonly ILogger<ProductsController> _logger;

    // Fake database
    private static readonly List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Laptop", Price = 999.99m },
        new Product { Id = 2, Name = "Mouse", Price = 29.99m },
        new Product { Id = 3, Name = "Keyboard", Price = 79.99m },
    };

    public ProductsController(
        ICacheService cache,
        ICacheKeyService cacheKey,
        ILogger<ProductsController> logger)
    {
        _cache = cache;
        _cacheKey = cacheKey;
        _logger = logger;
    }

    /// <summary>
    /// Lấy product theo ID - Demo cache GET
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CacheResponse<Product>>> GetProduct(int id)
    {
        var cacheKey = _cacheKey.GetCacheKey("product", id);

        // Thử lấy từ cache trước
        var cached = await _cache.GetAsync<Product>(cacheKey);

        if (cached is not null)
        {
            _logger.LogInformation("Cache HIT for key: {Key}", cacheKey);
            return Ok(new CacheResponse<Product>
            {
                Data = cached,
                FromCache = true,
                CacheKey = cacheKey
            });
        }

        // Cache miss - lấy từ "database"
        _logger.LogInformation("Cache MISS for key: {Key}", cacheKey);
        var product = _products.FirstOrDefault(p => p.Id == id);

        if (product is null)
            return NotFound($"Product with ID {id} not found");

        // Lưu vào cache với thời gian hết hạn 5 phút
        await _cache.SetAsync(cacheKey, product, TimeSpan.FromMinutes(5));
        _logger.LogInformation("Cached product {Id} with key: {Key}", id, cacheKey);

        return Ok(new CacheResponse<Product>
        {
            Data = product,
            FromCache = false,
            CacheKey = cacheKey
        });
    }

    /// <summary>
    /// Lấy tất cả products - Demo cache list
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<CacheResponse<List<Product>>>> GetAllProducts()
    {
        const string cacheKey = "products-all";

        var cached = await _cache.GetAsync<List<Product>>(cacheKey);

        if (cached is not null)
        {
            _logger.LogInformation("Cache HIT for all products");
            return Ok(new CacheResponse<List<Product>>
            {
                Data = cached,
                FromCache = true,
                CacheKey = cacheKey
            });
        }

        _logger.LogInformation("Cache MISS for all products");

        // Simulate delay từ database
        await Task.Delay(500);

        await _cache.SetAsync(cacheKey, _products, TimeSpan.FromMinutes(2));

        return Ok(new CacheResponse<List<Product>>
        {
            Data = _products,
            FromCache = false,
            CacheKey = cacheKey
        });
    }

    /// <summary>
    /// Xóa cache của product theo ID
    /// </summary>
    [HttpDelete("cache/{id:int}")]
    public async Task<IActionResult> InvalidateCache(int id)
    {
        var cacheKey = _cacheKey.GetCacheKey("product", id);
        await _cache.RemoveAsync(cacheKey);

        _logger.LogInformation("Cache INVALIDATED for key: {Key}", cacheKey);

        return Ok(new { Message = $"Cache invalidated for key: {cacheKey}" });
    }

    /// <summary>
    /// Xóa tất cả cache products
    /// </summary>
    [HttpDelete("cache")]
    public async Task<IActionResult> InvalidateAllCache()
    {
        // Xóa cache của từng product
        foreach (var product in _products)
        {
            var key = _cacheKey.GetCacheKey("product", product.Id);
            await _cache.RemoveAsync(key);
        }

        // Xóa cache list
        await _cache.RemoveAsync("products-all");

        _logger.LogInformation("All product caches INVALIDATED");

        return Ok(new { Message = "All product caches invalidated" });
    }

    /// <summary>
    /// Refresh cache - gia hạn thời gian sống
    /// </summary>
    [HttpPost("cache/{id:int}/refresh")]
    public async Task<IActionResult> RefreshCache(int id)
    {
        var cacheKey = _cacheKey.GetCacheKey("product", id);
        await _cache.RefreshAsync(cacheKey);

        _logger.LogInformation("Cache REFRESHED for key: {Key}", cacheKey);

        return Ok(new { Message = $"Cache refreshed for key: {cacheKey}" });
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class CacheResponse<T>
{
    public T? Data { get; set; }
    public bool FromCache { get; set; }
    public string CacheKey { get; set; } = string.Empty;
}
