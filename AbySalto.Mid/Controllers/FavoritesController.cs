using System.Security.Claims;
using AbySalto.Mid.Application.Services;
using AbySalto.Mid.Statics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Abysalto.Mid.Controller;

[ApiController]
[Route("api/favorites")]
[Authorize]
public sealed class FavoritesController(FavoriteService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var userId = UserContext.GetUserId(User);

        var products = await service.GetAsync(userId, cancellationToken);

        var result = products.Select(p => new
        {
            p.Id,
            p.Name,
            p.Price,
            p.Description
        });

        return Ok(result);
    }

    [HttpPost("{productId:guid}")]
    public async Task<IActionResult> Add(Guid productId, CancellationToken cancellationToken)
    {
        var userId = UserContext.GetUserId(User);

        await service.AddAsync(userId, productId, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> Remove(Guid productId, CancellationToken cancellationToken)
    {
        var userId = UserContext.GetUserId(User);

        await service.RemoveAsync(userId, productId, cancellationToken);

        return NoContent();
    }
}