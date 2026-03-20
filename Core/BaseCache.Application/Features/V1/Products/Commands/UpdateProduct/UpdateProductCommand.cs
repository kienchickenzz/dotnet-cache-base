namespace BaseCache.Application.Features.V1.Products.Commands.UpdateProduct;

using BaseCache.Application.Common.Messaging;


public sealed record UpdateProductCommand(
    int Id,
    string Name,
    string? Description,
    decimal Price) : ICommand<int>;
