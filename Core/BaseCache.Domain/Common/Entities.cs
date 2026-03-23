namespace BaseCache.Domain.Common;


public interface IAggregateRoot
{
}

public abstract class BaseEntity : IAuditableEntity, ISoftDelete
{
    public int Id { get; init; }
    public int CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public int LastModifiedBy { get; set; }
    public DateTime LastModifiedOn { get; set; }
    public DateTime? DeletedOn { get; set; }
    public int? DeletedBy { get; set; }
}

public abstract class BaseEntityRoot : BaseEntity, IAggregateRoot
{
}
