namespace BaseCache.Application.Common;


public interface IAuditResponse : IResponse
{
    public int CreatedBy { get; init; }
    public DateTime CreatedOn { get; init; }
    public int LastModifiedBy { get; init; }
    public DateTime? LastModifiedOn { get; init; }
    public DateTime? DeletedOn { get; init; }
    public int? DeletedBy { get; init; }
}
