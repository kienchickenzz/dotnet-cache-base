namespace BaseCache.Application.Common.Messaging;

using MediatR;

using BaseCache.Domain.Common;


public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
