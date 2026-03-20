/**
 * Repository implementation for Product aggregate.
 *
 * <p>Inherits all CRUD operations from base Repository.
 * Add domain-specific query methods here if needed.</p>
 */
namespace BaseCache.Persistence.Repositories;

using BaseCache.Application.Common.ApplicationServices.Persistence;
using BaseCache.Domain.AggregatesModels.Products;
using BaseCache.Persistence.Common;
using BaseCache.Persistence.DatabaseContext;


public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
