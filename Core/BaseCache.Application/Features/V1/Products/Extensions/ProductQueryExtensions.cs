/**
 * Extension methods for Product queries.
 *
 * <p>Provides reusable filter, sort, and projection operations
 * specific to Product entity. Replaces Specification pattern.</p>
 */
namespace BaseCache.Application.Features.V1.Products.Extensions;

using BaseCache.Application.Features.V1.Products.Models.Responses;
using BaseCache.Domain.AggregatesModels.Products;


public static class ProductQueryExtensions
{
    /// <summary>
    /// Tìm kiếm theo keyword trong Name hoặc Description.
    /// </summary>
    public static IQueryable<Product> WhereKeywordMatches(this IQueryable<Product> query, string? keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return query;

        return query.Where(p =>
            p.Name.Contains(keyword) ||
            (p.Description != null && p.Description.Contains(keyword)));
    }

    /// <summary>
    /// Sắp xếp theo thời gian tạo mới nhất.
    /// </summary>
    public static IQueryable<Product> OrderByNewest(this IQueryable<Product> query)
        => query.OrderByDescending(p => p.CreatedOn);

    /// <summary>
    /// Projection sang ProductResponse DTO.
    /// </summary>
    public static IQueryable<ProductResponse> SelectAsResponse(this IQueryable<Product> query)
        => query.Select(p => new ProductResponse
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            CreatedBy = p.CreatedBy,
            CreatedOn = p.CreatedOn,
            LastModifiedBy = p.LastModifiedBy,
            LastModifiedOn = p.LastModifiedOn,
            DeletedOn = p.DeletedOn,
            DeletedBy = p.DeletedBy
        });
}
