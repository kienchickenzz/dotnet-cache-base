namespace BaseCache.Application.Common.Exceptions;

using System.Net;

using BaseCache.Domain.Common;


public class InternalServerException : DomainException
{
    public InternalServerException(string message, List<Error>? errors = default)
        : base(message, errors, HttpStatusCode.InternalServerError)
    {
    }
}
