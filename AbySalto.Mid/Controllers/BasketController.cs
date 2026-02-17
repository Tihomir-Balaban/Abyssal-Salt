using AbySalto.Mid.Application.Basket;
using AbySalto.Mid.Application.Common;
using AbySalto.Mid.Application.Services;
using AbySalto.Mid.Domain.Entities;
using AbySalto.Mid.Infrastructure.Security;
using AbySalto.Mid.Statics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Abysalto.Mid.Controller;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public sealed class BasketController(BasketService service) : ControllerBase
{
    [HttpPost("me")]
    public async Task<ActionResult<Basket>> Create(CancellationToken cancellationToken)
    {
        var userId = UserContext.GetUserIdOrNull(User);

        var basket = await service.CreateAsync(userId, cancellationToken);

        return Ok(new { basket.Id });
    }

    [HttpGet("{basketId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get(Guid basketId, CancellationToken cancellationToken)
    {
        var basket = await service.GetAsync(basketId, cancellationToken);
        return basket is null ? NotFound() : Ok(basket);
    }

    [HttpPost("{basketId:guid}/items")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddItem(
        Guid basketId,
        [FromBody] AddItemRequest request,
        CancellationToken cancellationToken)
    {
        var ok = await service.AddItemAsync(basketId, request.ProductId, request.Quantity, cancellationToken);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{basketId:guid}/items/{productId:guid}")]
    public async Task<IActionResult> RemoveItem(
        Guid basketId,
        Guid productId,
        CancellationToken cancellationToken)
    {
        var ok = await service.RemoveItemAsync(basketId, productId, cancellationToken);
        return ok ? NoContent() : NotFound();
    }

    [HttpPut("{basketId:guid}/items/{productId:guid}")]
    public async Task<IActionResult> SetQuantity(
        Guid basketId,
        Guid productId,
        [FromBody] SetQuantityRequest request,
        CancellationToken cancellationToken)
    {
        var ok = await service.SetQuantityAsync(basketId, productId, request.Quantity, cancellationToken);
        return ok ? NoContent() : NotFound();
    }
}