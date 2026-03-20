/**
 * Extension methods for IQueryable - pagination and execution helpers.
 *
 * <p>Provides reusable query operations for all entities,
 * including pagination with PaginationResponse wrapper.</p>
 */
namespace BaseCache.Application.Common.Extensions;

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

using BaseCache.Application.Common.Models;


public static class QueryableExtensions
{
    /// <summary>
    /// Áp dụng pagination lên query (skip/take).
    /// </summary>
    public static IQueryable<T> ApplyPaging<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize)
    {
        return query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }

    /// <summary>
    /// Thực thi query với projection và trả về PaginationResponse.
    /// </summary>
    public static async Task<PaginationResponse<TResult>> ToPaginatedListAsync<T, TResult>(
        this IQueryable<T> query,
        Expression<Func<T, TResult>> selector,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);

        var data = await query
            .ApplyPaging(pageNumber, pageSize)
            .Select(selector)
            .ToListAsync(cancellationToken);

        return new PaginationResponse<TResult>(data, totalCount, pageNumber, pageSize);
    }

    /// <summary>
    /// Thực thi query và trả về PaginationResponse (không cần projection).
    /// </summary>
    public static async Task<PaginationResponse<T>> ToPaginatedListAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);

        var data = await query
            .ApplyPaging(pageNumber, pageSize)
            .ToListAsync(cancellationToken);

        return new PaginationResponse<T>(data, totalCount, pageNumber, pageSize);
    }
}
