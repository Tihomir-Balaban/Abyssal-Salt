using System.Security.Claims;
using AbySalto.Mid.Application.Basket;
using AbySalto.Mid.Application.Common;
using AbySalto.Mid.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Abysalto.Mid.Controller;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class BasketController(BasketService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = string.IsNullOrWhiteSpace(userIdValue) ? (Guid?)null : Guid.Parse(userIdValue);

        var basket = await service.CreateAsync(userId, cancellationToken);
        
        return Ok(new { basket.Id });
    }

    [HttpGet("{basketId:guid}")]
    public async Task<IActionResult> Get(Guid basketId, CancellationToken cancellationToken)
    {
        var basket = await service.GetAsync(basketId, cancellationToken);
        return basket is null ? NotFound() : Ok(basket);
    }

    [HttpPost("{basketId:guid}/items")]
    public async Task<IActionResult> AddItem(Guid basketId, [FromBody] AddItemRequest request, CancellationToken cancellationToken)
    {
        var ok = await service.AddItemAsync(basketId, request.ProductId, request.Quantity, cancellationToken);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{basketId:guid}/items/{productId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid basketId, Guid productId, CancellationToken cancellationToken)
    {
        var ok = await service.RemoveItemAsync(basketId, productId, cancellationToken);
        return ok ? NoContent() : NotFound();
    }

    [HttpPut("{basketId:guid}/items/{productId:guid}")]
    public async Task<IActionResult> SetQuantity(Guid basketId, Guid productId, [FromBody] SetQuantityRequest request, CancellationToken cancellationToken)
    {
        var ok = await service.SetQuantityAsync(basketId, productId, request.Quantity, cancellationToken);
        return ok ? NoContent() : NotFound();
    }
}