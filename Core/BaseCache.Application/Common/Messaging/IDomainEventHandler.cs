namespace BaseCache.Application.Common.Messaging;

using MediatR;

using BaseCache.Domain.Common;


public interface IDomainEventHandler<TEvent> : INotificationHandler<TEvent>
    where TEvent : IDomainEvent
{
}
