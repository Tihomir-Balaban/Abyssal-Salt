using AbySalto.Mid.Application.Services;
using AbySalto.Mid.Statics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AbySalto.Mid.WebApi.Controllers;

[ApiController]
[Route("api/favorites")]
[Authorize]
public sealed class FavoritesController(FavoriteService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Add(Guid productId, CancellationToken cancellationToken)
    {
        var userId = UserContext.GetUserId(User);

        try
        {
            await service.AddAsync(userId, productId, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex) when (ex.Message == "Product not found.")
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex) when (ex.Message == "Already favorited.")
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpDelete("{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remove(Guid productId, CancellationToken cancellationToken)
    {
        var userId = UserContext.GetUserId(User);

        try
        {
            await service.RemoveAsync(userId, productId, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex) when (ex.Message == "Favorite not found.")
        {
            return NotFound(new { error = ex.Message });
        }
    }
}