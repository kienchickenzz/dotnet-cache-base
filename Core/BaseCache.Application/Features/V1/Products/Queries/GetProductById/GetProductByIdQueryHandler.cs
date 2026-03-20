/**
 * Handler for GetProductByIdQuery - retrieves a single product by Id.
 *
 * <p>Uses LINQ with projection extension instead of Specification pattern.</p>
 */
namespace BaseCache.Application.Features.V1.Products.Queries.GetProductById;

using Microsoft.EntityFrameworkCore;

using BaseCache.Application.Common.ApplicationServices.Persistence;
using BaseCache.Application.Common.Messaging;
using BaseCache.Application.Features.V1.Products.Extensions;
using BaseCache.Application.Features.V1.Products.Models.Responses;
using BaseCache.Domain.AggregatesModels.Products;
using BaseCache.Domain.Common;


public sealed class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<Result<ProductResponse>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.Query
            .Where(p => p.Id == request.Id)
            .SelectAsResponse()
            .FirstOrDefaultAsync(cancellationToken);

        return product is not null
            ? Result.Success(product)
            : Result.Failure<ProductResponse>(ProductErrors.NotFound);
    }
}
