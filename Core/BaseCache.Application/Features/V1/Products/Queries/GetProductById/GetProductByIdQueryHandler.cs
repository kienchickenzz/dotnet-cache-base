/**
 * Handler for GetProductByIdQuery - retrieves a single product by Id with Cache-Aside pattern.
 *
 * <p>Uses caching to reduce database load for frequently accessed products.
 * Cache is checked first; on miss, data is fetched from DB via LINQ projection and cached.</p>
 */
namespace BaseCache.Application.Features.V1.Products.Queries.GetProductById;

using Microsoft.EntityFrameworkCore;

using BaseCache.Application.Common.ApplicationServices.Caching;
using BaseCache.Application.Common.ApplicationServices.Persistence;
using BaseCache.Application.Common.Messaging;
using BaseCache.Application.Features.V1.Products.Extensions;
using BaseCache.Application.Features.V1.Products.Models.Responses;
using BaseCache.Domain.AggregatesModels.Products;
using BaseCache.Domain.Common;


/// <summary>
/// Handles <see cref="GetProductByIdQuery"/> using Cache-Aside pattern.
/// </summary>
public sealed class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cache;
    private readonly ICacheKeyService _cacheKeys;

    public GetProductByIdQueryHandler(
        IProductRepository productRepository,
        ICacheService cache,
        ICacheKeyService cacheKeys)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _cacheKeys = cacheKeys ?? throw new ArgumentNullException(nameof(cacheKeys));
    }

    public async Task<Result<ProductResponse>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = _cacheKeys.GetCacheKey(nameof(Product), request.Id);

        // Cache-Aside: try cache first, fallback to DB on miss
        var product = await _cache.GetOrSetAsync(
            cacheKey,
            () => _FetchFromDatabaseAsync(request.Id, cancellationToken),
            cancellationToken: cancellationToken);

        return product is not null
            ? Result.Success(product)
            : Result.Failure<ProductResponse>(ProductErrors.NotFound);
    }

    /// <summary>
    /// Fetches product from database using pure LINQ projection.
    /// </summary>
    private async Task<ProductResponse?> _FetchFromDatabaseAsync(int id, CancellationToken cancellationToken)
    {
        return await _productRepository.Query
            .Where(p => p.Id == id)
            .SelectAsResponse()
            .FirstOrDefaultAsync(cancellationToken);
    }
}
