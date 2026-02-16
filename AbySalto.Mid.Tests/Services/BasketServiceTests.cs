using AbySalto.Mid.Application.Services;
using AbySalto.Mid.Domain.Entities;
using AbySalto.Mid.Tests.TestDoubles;
using Xunit;

namespace AbySalto.Mid.Tests.Services;

public sealed class BasketServiceTests
{
    [Fact]
    public async Task AddItemAsync_WhenQuantityIsNonPositive_ReturnsFalse()
    {
        // Arrange
        var baskets = new InMemoryBasketRepository();
        var service = new BasketService(baskets);

        var basketId = Guid.NewGuid();

        // Act
        var resultZero = await service.AddItemAsync(basketId, Guid.NewGuid(), 0, CancellationToken.None);
        var resultNegative = await service.AddItemAsync(basketId, Guid.NewGuid(), -1, CancellationToken.None);

        // Assert
        Assert.False(resultZero);
        Assert.False(resultNegative);
    }

    [Fact]
    public async Task AddItemAsync_WhenSameProductAddedTwice_MergesQuantities()
    {
        // Arrange
        var baskets = new InMemoryBasketRepository();
        var service = new BasketService(baskets);

        var basket = new Basket { Id = Guid.NewGuid() };
        
        // Act
        await baskets.AddAsync(basket, CancellationToken.None);

        var productId = Guid.NewGuid();

        // Assert
        Assert.True(await service.AddItemAsync(basket.Id, productId, 2, CancellationToken.None));
        Assert.True(await service.AddItemAsync(basket.Id, productId, 3, CancellationToken.None));

        var updated = await baskets.GetByIdAsync(basket.Id, CancellationToken.None);
        Assert.NotNull(updated);
        Assert.Single(updated!.Items);
        Assert.Equal(5, updated.Items[0].Quantity);
    }

    [Fact]
    public async Task SetQuantityAsync_WhenQuantityBecomesNonPositive_RemovesItem()
    {
        // Arrange
        var baskets = new InMemoryBasketRepository();
        var service = new BasketService(baskets);

        var productId = Guid.NewGuid();
        var basket = new Basket
        {
            Id = Guid.NewGuid(),
            Items = new List<BasketItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    BasketId = Guid.NewGuid(),
                    ProductId = productId,
                    Quantity = 2
                }
            }
        };
        
        basket.Items[0].BasketId = basket.Id;

        // Act
        await baskets.AddAsync(basket, CancellationToken.None);
        
        // Assert
        Assert.True(await service.SetQuantityAsync(basket.Id, productId, 0, CancellationToken.None));

        var updated = await baskets.GetByIdAsync(basket.Id, CancellationToken.None);
        Assert.NotNull(updated);
        Assert.Empty(updated!.Items);
    }
}
