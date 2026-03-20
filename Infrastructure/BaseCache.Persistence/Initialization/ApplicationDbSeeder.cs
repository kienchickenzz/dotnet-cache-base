// -----------------------------------------------------------------------------
// <summary>
//     Seeder để khởi tạo dữ liệu mẫu cho database.
// </summary>
// -----------------------------------------------------------------------------
namespace BaseCache.Persistence.Initialization;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using BaseCache.Domain.AggregatesModels.Products;
using BaseCache.Persistence.DatabaseContext;


internal static class ApplicationDbSeeder
{
    /// <summary>
    /// Entry point để seed toàn bộ database.
    /// </summary>
    public static async Task SeedDatabaseAsync(ApplicationDbContext dbContext, ILogger logger, CancellationToken cancellationToken)
    {
        await SeedProductsAsync(dbContext, logger, cancellationToken);
    }

    /// <summary>
    /// Seed sample products nếu bảng trống.
    /// </summary>
    private static async Task SeedProductsAsync(ApplicationDbContext dbContext, ILogger logger, CancellationToken cancellationToken)
    {
        if (await dbContext.Products.AnyAsync(cancellationToken))
        {
            logger.LogInformation("Products table already has data. Skipping seed.");
            return;
        }

        logger.LogInformation("Seeding Products for database...");

        var products = new List<Product>
        {
            Product.Create("Laptop", "High performance laptop for developers", 999.99m),
            Product.Create("Mouse", "Wireless ergonomic mouse", 29.99m),
            Product.Create("Keyboard", "Mechanical keyboard with RGB", 79.99m),
            Product.Create("Monitor", "27 inch 4K display", 399.99m),
            Product.Create("Headphones", "Noise cancelling wireless headphones", 199.99m)
        };

        await dbContext.Products.AddRangeAsync(products, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Seeded {Count} products successfully.", products.Count);
    }
}
