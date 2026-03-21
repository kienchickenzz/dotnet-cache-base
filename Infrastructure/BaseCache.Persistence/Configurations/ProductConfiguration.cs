/**
 * ProductConfiguration defines EF Core mapping for the Product entity.
 *
 * <p>Configures database column types, constraints, and indexes for Product table.</p>
 */
namespace BaseCache.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using BaseCache.Domain.AggregatesModels.Products;


/// <summary>
/// EF Core configuration for <see cref="Product"/> entity.
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        // Precision 18, Scale 2 → hỗ trợ giá trị lên đến 9999999999999999.99
        builder.Property(p => p.Price)
            .HasPrecision(18, 2);
    }
}
