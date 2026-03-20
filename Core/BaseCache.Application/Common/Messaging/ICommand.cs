namespace BaseCache.Application.Common.Messaging;

using MediatR;

using BaseCache.Domain.Common;


public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}
