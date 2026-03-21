/**
 * Handler for DeleteProductCommand - performs soft-delete on a product and invalidates cache.
 *
 * <p>Uses soft-delete pattern (sets DeletedOn timestamp) rather than hard delete,
 * preserving data for audit/recovery purposes. Global query filter excludes soft-deleted entities.</p>
 */
namespace BaseCache.Application.Features.V1.Products.Commands.DeleteProduct;

using BaseCache.Application.Common.ApplicationServices.Caching;
using BaseCache.Application.Common.ApplicationServices.Persistence;
using BaseCache.Application.Common.Messaging;
using BaseCache.Domain.AggregatesModels.Products;
using BaseCache.Domain.Common;


/// <summary>
/// Handles <see cref="DeleteProductCommand"/> with soft-delete and cache invalidation.
/// </summary>
public sealed class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand, int>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cache;
    private readonly ICacheKeyService _cacheKeys;

    public DeleteProductCommandHandler(
        IProductRepository productRepository,
        ICacheService cache,
        ICacheKeyService cacheKeys)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _cacheKeys = cacheKeys ?? throw new ArgumentNullException(nameof(cacheKeys));
    }

    public async Task<Result<int>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Result.Failure<int>(ProductErrors.NotFound);

        // Soft-delete: sets DeletedOn timestamp, filtered by global query filter
        await _productRepository.SoftDeleteAsync(product, cancellationToken);
        // Note: SaveChanges handled by TransactionPipelineBehavior

        // Invalidate cache
        var cacheKey = _cacheKeys.GetCacheKey(nameof(Product), product.Id);
        await _cache.RemoveAsync(cacheKey, cancellationToken);

        return product.Id;
    }
}
