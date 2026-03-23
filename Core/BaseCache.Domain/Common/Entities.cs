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
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList();
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
