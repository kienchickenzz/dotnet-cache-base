/**
 * Generic repository implementation using EF Core.
 *
 * <p>Provides basic CRUD operations and IQueryable access
 * for flexible LINQ queries. Child repositories only need
 * to specify the entity type.</p>
 */
namespace BaseCleanArchitecture.Persistence.Common;

using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using BaseCleanArchitecture.Application.Common.ApplicationServices.Persistence;
using BaseCleanArchitecture.Domain.Common;
using BaseCleanArchitecture.Persistence.DatabaseContext;


public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : BaseEntity
{
    protected readonly ApplicationDbContext DbContext;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        DbSet = dbContext.Set<TEntity>();
    }

    // === READ ===

    /// <summary>
    /// Expose IQueryable để handlers compose LINQ queries.
    /// </summary>
    public IQueryable<TEntity> Query => DbSet.AsQueryable();

    /// <summary>
    /// Lấy entity theo Id.
    /// </summary>
    public virtual async Task<TEntity?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    // === WRITE ===

    /// <summary>
    /// Thêm entity mới.
    /// </summary>
    public virtual async Task<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    /// <summary>
    /// Thêm nhiều entities.
    /// </summary>
    public virtual async Task AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
    }

    /// <summary>
    /// Đánh dấu entity đã cập nhật.
    /// </summary>
    public virtual Task UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Xóa entity (hard delete).
    /// </summary>
    public virtual Task DeleteAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Soft delete - set DeletedOn timestamp.
    /// </summary>
    public virtual Task SoftDeleteAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        entity.DeletedOn = DateTime.UtcNow;
        DbSet.Update(entity);
        return Task.CompletedTask;
    }

    // === UTILITIES ===

    /// <summary>
    /// Đếm entities theo điều kiện (hoặc tất cả nếu predicate = null).
    /// </summary>
    public virtual async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        return predicate is null
            ? await DbSet.CountAsync(cancellationToken)
            : await DbSet.CountAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Kiểm tra có entity nào thỏa mãn điều kiện không.
    /// </summary>
    public virtual async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(predicate, cancellationToken);
    }
}
