namespace BaseCache.Application.Features.V1.Products.Commands.UpdateProduct;

using BaseCache.Application.Common.Messaging;
using BaseCache.Application.Common.ApplicationServices.Persistence;
using BaseCache.Domain.AggregatesModels.Products;
using BaseCache.Domain.Common;


public sealed class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, int>
{
    private readonly IProductRepository _productRepository;

    public UpdateProductCommandHandler(
        IProductRepository productRepository
    )
    {
        _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
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

        return product.Id;
    }
}
