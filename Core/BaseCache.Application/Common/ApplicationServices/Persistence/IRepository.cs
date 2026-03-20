/**
 * Generic repository interface for basic CRUD operations.
 *
 * <p>Exposes IQueryable for flexible LINQ queries in handlers.
 * Infrastructure layer implements this with EF Core DbContext.</p>
 */
namespace BaseCache.Application.Common.ApplicationServices.Persistence;

using System.Linq.Expressions;

using BaseCache.Domain.Common;


public interface IRepository<TEntity> where TEntity : BaseEntity
{
    // === READ ===

    /// <summary>
    /// Truy cập IQueryable để compose LINQ queries linh hoạt.
    /// </summary>
    IQueryable<TEntity> Query { get; }

    /// <summary>
    /// Lấy entity theo Id, trả về null nếu không tìm thấy.
    /// </summary>
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    // === WRITE ===

    /// <summary>
    /// Thêm entity mới vào repository.
    /// </summary>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Thêm nhiều entities vào repository.
    /// </summary>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Đánh dấu entity đã được cập nhật.
    /// </summary>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Xóa entity khỏi repository (hard delete).
    /// </summary>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft delete - đánh dấu entity đã bị xóa.
    /// </summary>
    Task SoftDeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    // === UTILITIES ===

    /// <summary>
    /// Đếm số lượng entities thỏa mãn điều kiện.
    /// </summary>
    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Kiểm tra có entity nào thỏa mãn điều kiện không.
    /// </summary>
    Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
}
