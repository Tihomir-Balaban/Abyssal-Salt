using AbySalto.Mid.Application.Products;
using AbySalto.Mid.Application.Requests.Products;
using Microsoft.AspNetCore.Mvc;

namespace Abysalto.Mid.Controller;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductController(ProductQueries queries) : ControllerBase
{
    [HttpGet]
    public Task<IActionResult> GetPaged(
        [FromQuery] GetProductsRequest request,
        CancellationToken cancellationToken)
        => GetPagedInternal(request, cancellationToken);

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var product = await queries.GetByIdAsync(id, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    private async Task<IActionResult> GetPagedInternal(GetProductsRequest request, CancellationToken cancellationToken)
    {
        var result = await queries.GetPagedAsync(request, cancellationToken);
        return Ok(result);
    }
}