namespace BaseCache.Application.Features.V1.Products.Queries.GetProductById;

using BaseCache.Application.Common.Messaging;
using BaseCache.Application.Features.V1.Products.Models.Responses;


public sealed record GetProductByIdQuery(int Id) : IQuery<ProductResponse>;
