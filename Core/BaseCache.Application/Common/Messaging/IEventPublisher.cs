namespace BaseCache.Application.Common.Messaging;

using BaseCache.Domain.Common;


public interface IEventPublisher
{
    Task PublishAsync(IDomainEvent @event);
}
