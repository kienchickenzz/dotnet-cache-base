namespace BaseCache.Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected readonly ISender Sender;

    protected BaseApiController(ISender sender) => Sender = sender;
}
