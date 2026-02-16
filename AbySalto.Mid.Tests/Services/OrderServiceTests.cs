using AbySalto.Mid.Application.Services;
using AbySalto.Mid.Domain.Entities;
using AbySalto.Mid.Tests.TestDoubles;
using Xunit;

namespace AbySalto.Mid.Tests.Services;

public sealed class OrderServiceTests
{
    [Fact]
    public async Task CheckoutAsync_WhenBasketIsEmpty_Throws()
    {
        // Arrange
        var baskets = new InMemoryBasketRepository();
        var products = new InMemoryProductRepository();
        var orders = new InMemoryOrderRepository();

        var service = new OrderService(baskets, products, orders);

        var basket = new Basket { Id = Guid.NewGuid() };
        
        // Act
        await baskets.AddAsync(basket, CancellationToken.None);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CheckoutAsync(basket.Id, userId: null, CancellationToken.None));

        // Assert
        Assert.Equal("Basket is empty.", ex.Message);
    }

    [Fact]
    public async Task CheckoutAsync_CreatesOrderWithCorrectTotal_AndClearsBasket()
    {
        // Arrange
        var baskets = new InMemoryBasketRepository();
        var products = new InMemoryProductRepository();
        var orders = new InMemoryOrderRepository();

        var service = new OrderService(baskets, products, orders);

        var userId = Guid.NewGuid();
        var p1 = new Product { Id = Guid.NewGuid(), Name = "A", Price = 10m };
        var p2 = new Product { Id = Guid.NewGuid(), Name = "B", Price = 2.5m };
        products.Seed(p1, p2);

        var basket = new Basket
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Items =
            [
                new BasketItem { Id = Guid.NewGuid(), BasketId = Guid.NewGuid(), ProductId = p1.Id, Quantity = 2 },
                new BasketItem { Id = Guid.NewGuid(), BasketId = Guid.NewGuid(), ProductId = p2.Id, Quantity = 4 },
            ]
        };

        foreach (var item in basket.Items)
            item.BasketId = basket.Id;

        
        // Act
        await baskets.AddAsync(basket, CancellationToken.None);

        var order = await service.CheckoutAsync(basket.Id, userId, CancellationToken.None);

        // Assert
        Assert.Equal(userId, order.UserId);
        Assert.Equal(2, order.Items.Count);
        Assert.Equal(10m * 2 + 2.5m * 4, order.Total);

        var savedBasket = await baskets.GetByIdAsync(basket.Id, CancellationToken.None);
        Assert.NotNull(savedBasket);
        Assert.Empty(savedBasket!.Items);

        var reloadedOrder = await orders.GetByIdAsync(order.Id, CancellationToken.None);
        Assert.NotNull(reloadedOrder);
        Assert.Equal(order.Total, reloadedOrder!.Total);
    }

    [Fact]
    public async Task PlaceOrderAsync_WhenBasketDoesNotExistForUser_Throws()
    {
        // Arrange
        var baskets = new InMemoryBasketRepository();
        var products = new InMemoryProductRepository();
        var orders = new InMemoryOrderRepository();

        var service = new OrderService(baskets, products, orders);

        // Act
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.PlaceOrderAsync(Guid.NewGuid(), CancellationToken.None));

        // Assert
        Assert.Equal("Basket not found for user.", ex.Message);
    }
}
