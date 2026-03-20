namespace BaseCache.Persistence.DatabaseContext;

using BaseCache.Application.Common.ApplicationServices.Persistence;
using BaseCache.Application.Common.Exceptions;
using BaseCache.Domain.AggregatesModels.Products;
using BaseCache.Domain.Common;
using BaseCache.Persistence.Extentions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;


public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // QueryFilters need to be applied before base.OnModelCreating
        builder.AppendGlobalQueryFilter<ISoftDelete>(s => s.DeletedOn == null);

        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _UpdateAuditableEntities();

            int result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Concurrency exception occurred.", ex);
        }
    }

    private void _UpdateAuditableEntities()
    {
        IEnumerable<EntityEntry<BaseEntity>> entries = 
                 ChangeTracker
                .Entries<BaseEntity>();

        foreach (EntityEntry<BaseEntity> entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(a => a.CreatedOn)
                    .CurrentValue = DateTime.UtcNow;
            }

            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(a => a.LastModifiedOn)
                    .CurrentValue = DateTime.UtcNow;
            }
        }
    }
}
