/**
 * Handler for UpdateProductCommand - updates product details and invalidates cache.
 *
 * <p>After updating, the cached entry is removed to ensure subsequent reads
 * fetch fresh data from the database (cache invalidation strategy).</p>
 */
namespace BaseCache.Application.Features.V1.Products.Commands.UpdateProduct;

using BaseCache.Application.Common.ApplicationServices.Caching;
using BaseCache.Application.Common.ApplicationServices.Persistence;
using BaseCache.Application.Common.Messaging;
using BaseCache.Domain.AggregatesModels.Products;
using BaseCache.Domain.Common;


/// <summary>
/// Handles <see cref="UpdateProductCommand"/> with cache invalidation.
/// </summary>
public sealed class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, int>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cache;
    private readonly ICacheKeyService _cacheKeys;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        ICacheService cache,
        ICacheKeyService cacheKeys)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _cacheKeys = cacheKeys ?? throw new ArgumentNullException(nameof(cacheKeys));
    }

    public async Task<Result<int>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        Product? product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Result.Failure<int>(ProductErrors.NotFound);

        // Update price only if it actually changed
        if (request.Price != product.Price)
        {
            product.UpdatePrice(request.Price);
        }

        // Prepare values for UpdateDetails
        string name = request.Name ?? product.Name;
        string? description = request.Description ?? product.Description;

        product.UpdateDetails(name, description);

        await _productRepository.UpdateAsync(product, cancellationToken);
        // Note: SaveChanges handled by TransactionPipelineBehavior

        // Invalidate cache to ensure consistency
        var cacheKey = _cacheKeys.GetCacheKey(nameof(Product), product.Id);
        await _cache.RemoveAsync(cacheKey, cancellationToken);

        return product.Id;
    }
}
