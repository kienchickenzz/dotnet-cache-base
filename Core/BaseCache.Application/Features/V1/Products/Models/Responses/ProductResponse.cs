namespace BaseCache.Application.Features.V1.Products.Models.Responses;

using BaseCache.Application.Common;


public sealed class ProductResponse : IAuditResponse
{
    public int Id { get; init; }
    public int CreatedBy { get; init; }
    public DateTime CreatedOn { get; init; }
    public int LastModifiedBy { get; init; }
    public DateTime? LastModifiedOn { get; init; }
    public DateTime? DeletedOn { get; init; }
    public int? DeletedBy { get; init; }

    public string Name { get; init; }
    public string Description { get; init; }
    public decimal Price { get; init; }
}
