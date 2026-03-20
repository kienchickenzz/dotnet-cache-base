namespace BaseCache.Application.Features.V1.Products.Commands.CreateProduct;

using BaseCache.Application.Common.Messaging;


public sealed record CreateProductCommand(
    string Name,
    string? Description,
    decimal Price)
    : ICommand<int>;
