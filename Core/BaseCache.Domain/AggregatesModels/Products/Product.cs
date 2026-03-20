namespace BaseCache.Domain.AggregatesModels.Products;

using BaseCache.Domain.Common;


public sealed class Product : BaseEntityRoot
{
    private Product(
        string name,
        string? description,
        decimal price)
    {
        Name = name;
        Description = description;
        Price = price;
    }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public static Product Create(
        string name,
        string? description,
        decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));

        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        var product = new Product(
            name,
            description,
            price);

        return product;
    }

    public void UpdateDetails(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));
            
        Name = name;
        Description = description;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("Price cannot be negative", nameof(newPrice));
            
        Price = newPrice;
    }
}
