namespace BaseCache.Application.Common.Exceptions;

using System.Net;

using BaseCache.Domain.Common;


public class BadRequestException : DomainException
{
    public BadRequestException(IEnumerable<Error>? errors)
        : base("Bad request encountered.", errors, HttpStatusCode.BadRequest)
    {
    }

    public BadRequestException(string message)
        : base(message, null, HttpStatusCode.BadRequest)
    {
    }
}
