namespace BaseCache.Application.Features.V1.Products.Commands.DeleteProduct;

using BaseCache.Application.Common.Messaging;
using BaseCache.Application.Common.ApplicationServices.Persistence;
using BaseCache.Domain.AggregatesModels.Products;
using BaseCache.Domain.Common;


public sealed class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand, int>
{
    private readonly IProductRepository _productRepository;
    public DeleteProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository= productRepository ?? throw new ArgumentNullException(nameof(productRepository));
    }

    public async Task<Result<int>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);
        if (product == null)
            return Result.Failure<int>(ProductErrors.NotFound);

        await _productRepository.DeleteAsync(product);

        return product.Id;
    }
}
