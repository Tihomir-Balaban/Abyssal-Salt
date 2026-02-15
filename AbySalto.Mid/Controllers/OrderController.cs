using AbySalto.Mid.Application.Services;
using AbySalto.Mid.Statics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Abysalto.Mid.Controller;

[ApiController]
[Route("api/orders")]
[Authorize]
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
}
