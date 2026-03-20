/**
 * Handler for GetProductsQuery - retrieves paginated list of products.
 *
 * <p>Uses LINQ extensions for filtering, sorting, and pagination
 * instead of Specification pattern.</p>
 */
namespace BaseCache.Application.Features.V1.Products.Queries.GetProducts;

using BaseCache.Application.Common.ApplicationServices.Persistence;
using BaseCache.Application.Common.Extensions;
using BaseCache.Application.Common.Messaging;
using BaseCache.Application.Common.Models;
using BaseCache.Application.Features.V1.Products.Extensions;
using BaseCache.Application.Features.V1.Products.Models.Responses;
using BaseCache.Domain.Common;


public sealed class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, PaginationResponse<ProductResponse>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<Result<PaginationResponse<ProductResponse>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _productRepository.Query
            .WhereKeywordMatches(request.Keyword)
            .OrderByNewest()
            .SelectAsResponse()
            .ToPaginatedListAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken);

        return Result.Success(result);
    }
}
