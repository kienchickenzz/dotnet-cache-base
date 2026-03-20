namespace BaseCache.Domain.Common;


public interface IAuditableEntity : ISoftDelete
{
    public int CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public int LastModifiedBy { get; set; }
    public DateTime LastModifiedOn { get; set; }
}
