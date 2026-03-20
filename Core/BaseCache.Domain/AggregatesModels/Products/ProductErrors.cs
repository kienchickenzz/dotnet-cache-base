namespace BaseCache.Domain.AggregatesModels.Products;

using BaseCache.Domain.Common;


public static class ProductErrors
{
    public static Error NotFound = new(
        "Product.NotFound",
        "Product not found!");
}
