using AbySalto.Mid.Application.Contracts;
using AbySalto.Mid.Domain.Entities;

namespace AbySalto.Mid.Application.Services;

public sealed class BasketService(IBasketRepository baskets)
{
    public Task<Domain.Entities.Basket> CreateAsync(CancellationToken cancellationToken)
        => baskets.CreateAsync(cancellationToken);

    public Task<Domain.Entities.Basket?> GetAsync(Guid basketId, CancellationToken cancellationToken)
        => baskets.GetByIdAsync(basketId, cancellationToken);

    public async Task<bool> AddItemAsync(
        Guid basketId,
        Guid productId,
        int quantity,
        CancellationToken cancellationToken)
    {
        if (quantity <= 0)
            return false;

        var basket = await baskets.GetByIdAsync(basketId, cancellationToken);
        if (basket is null)
            return false;

        var existing = basket.Items.FirstOrDefault(x => x.ProductId == productId);

        if (existing is null)
        {
            basket.Items.Add(new BasketItem
            {
                Id = Guid.NewGuid(),
                BasketId = basket.Id,
                ProductId = productId,
                Quantity = quantity
            });
        }
        else
        {
            existing.Quantity += quantity;
        }

        await baskets.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RemoveItemAsync(
        Guid basketId,
        Guid productId,
        CancellationToken cancellationToken)
    {
        var basket = await baskets.GetByIdAsync(basketId, cancellationToken);
        if (basket is null)
            return false;

        var existing = basket.Items.FirstOrDefault(x => x.ProductId == productId);
        if (existing is null)
            return false;

        basket.Items.Remove(existing);

        await baskets.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SetQuantityAsync(
        Guid basketId,
        Guid productId,
        int quantity,
        CancellationToken cancellationToken)
    {
        var basket = await baskets.GetByIdAsync(basketId, cancellationToken);
        if (basket is null)
            return false;

        var existing = basket.Items.FirstOrDefault(x => x.ProductId == productId);
        if (existing is null)
            return false;

        if (quantity <= 0)
        {
            basket.Items.Remove(existing);
        }
        else
        {
            existing.Quantity = quantity;
        }

        await baskets.SaveChangesAsync(cancellationToken);
        return true;
    }
}