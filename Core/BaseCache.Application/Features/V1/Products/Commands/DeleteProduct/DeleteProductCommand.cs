namespace BaseCache.Application.Features.V1.Products.Commands.DeleteProduct;

using BaseCache.Application.Common.Messaging;


public sealed record DeleteProductCommand(int Id) : ICommand<int>;
