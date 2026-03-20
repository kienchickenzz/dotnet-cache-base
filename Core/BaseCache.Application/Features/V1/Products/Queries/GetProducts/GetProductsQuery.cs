namespace BaseCache.Application.Features.V1.Products.Queries.GetProducts;

using BaseCache.Application.Common.Messaging;
using BaseCache.Application.Common.Models;
using BaseCache.Application.Features.V1.Products.Models.Responses;


public sealed class GetProductsQuery : PaginationFilter, IQuery<PaginationResponse<ProductResponse>>
{
}
