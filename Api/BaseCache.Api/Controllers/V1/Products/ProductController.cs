namespace BaseCache.Api.Controllers.Features.V1.Products;

using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using BaseCache.Application.Common.Exceptions;
using BaseCache.Application.Common.Models;
using BaseCache.Application.Features.V1.Products.Commands.CreateProduct;
using BaseCache.Application.Features.V1.Products.Commands.UpdateProduct;
using BaseCache.Application.Features.V1.Products.Commands.DeleteProduct;
using BaseCache.Application.Features.V1.Products.Models.Responses;
using BaseCache.Application.Features.V1.Products.Queries.GetProductById;
using BaseCache.Application.Features.V1.Products.Queries.GetProducts;
using BaseCache.Domain.Common;


[ApiVersion(1)]
public class ProductController : BaseApiController
{
    public ProductController(ISender sender) : base(sender)
    {
    }

    /// <summary>
    /// Get the paginated product list
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("paginated")]
    [ProducesResponseType(typeof(Result<PaginationResponse<ProductResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductPaginated([FromBody] GetProductsQuery request)
    {
        var result = await Sender.Send(request);
        return Ok(result);
    }

    /// <summary>
    /// Get product by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProductById(int id)
    {
        var result = await Sender.Send(new GetProductByIdQuery(id));

        if (result.IsFailure)
            throw new BadRequestException(new List<Error> { result.Error });

        return Ok(result);
    }

    /// <summary>
    /// Create product
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(Result<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            throw new BadRequestException(new List<Error> { result.Error });

        return CreatedAtAction(nameof(GetProductById), new { Id = result.Value }, result.Value);
    }

    /// <summary>
    /// Update product
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut]
    [ProducesResponseType(typeof(Result<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand request)
    {
        var result = await Sender.Send(request);

        if (result.IsFailure)
            throw new BadRequestException(new List<Error> { result.Error });

        return CreatedAtAction(nameof(GetProductById), new { Id = result.Value }, result.Value);
    }

    /// <summary>
    /// Delete product
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="BadRequestException"></exception>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await Sender.Send(new DeleteProductCommand(id));

        if (result.IsFailure)
            throw new BadRequestException(new List<Error> { result.Error });

        return Ok(result);
    }
}
