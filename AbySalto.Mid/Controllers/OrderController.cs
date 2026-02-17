using AbySalto.Mid.Application.Services;
using AbySalto.Mid.Statics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Abysalto.Mid.Controller;

[ApiController]
[Route("api/orders")]
[Authorize]
[Produces("application/json")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public sealed class OrderController(OrderService service) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var userId = UserContext.GetUserId(User);

        var order = await service.PlaceOrderAsync(userId, cancellationToken);

        return Ok(new
        {
            order.Id,
            order.CreatedAtUtc,
            Items = order.Items.Select(x => new
            {
                x.ProductId,
                x.ProductName,
                x.UnitPrice,
                x.Quantity
            }),
            Total = order.Items.Sum(x => x.UnitPrice * x.Quantity)
        });
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        var userId = UserContext.GetUserId(User);

        var orders = await service.GetForUserAsync(userId, cancellationToken);

        return Ok(orders.Select(o => new
        {
            o.Id,
            o.CreatedAtUtc,
            ItemsCount = o.Items.Sum(x => x.Quantity),
            Total = o.Items.Sum(x => x.UnitPrice * x.Quantity)
        }));
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetById(Guid orderId, CancellationToken cancellationToken)
    {
        var order = await service.GetByIdAsync(orderId, cancellationToken);
        if (order is null)
            return NotFound();

        var userId = UserContext.GetUserId(User);
        if (order.UserId != userId)
            return Forbid();

        return Ok(new
        {
            order.Id,
            order.CreatedAtUtc,
            Items = order.Items.Select(x => new
            {
                x.ProductId,
                x.ProductName,
                x.UnitPrice,
                x.Quantity
            }),
            Total = order.Items.Sum(x => x.UnitPrice * x.Quantity)
        });
    }
    
    [HttpPost("{basketId:guid}/checkout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Checkout(Guid basketId, CancellationToken cancellationToken)
    {
        // If you implemented IUserContext / claims extraction:
        Guid? userId = null;

        if (User.Identity?.IsAuthenticated == true)
        {
            var sub = User.FindFirst("sub")?.Value;
            if (Guid.TryParse(sub, out var parsed))
                userId = parsed;
        }

        var order = await service.CheckoutAsync(basketId, userId, cancellationToken);

        return Ok(order);
    }
}
