using AbySalto.Mid.Application.Contracts;
using AbySalto.Mid.Domain.Entities;

namespace AbySalto.Mid.Application.Services;

public sealed class OrderService(
    IBasketRepository basketRepository,
    IProductRepository productRepository,
    IOrderRepository orderRepository)
{
    public async Task<Order> CheckoutAsync(Guid basketId, Guid? userId, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetByIdAsync(basketId, cancellationToken);
        if (basket is null)
            throw new InvalidOperationException($"Basket '{basketId}' was not found.");

        if (basket.Items.Count == 0)
            throw new InvalidOperationException("Basket is empty.");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAtUtc = DateTime.UtcNow,
        };

        decimal total = 0m;

        foreach (var item in basket.Items)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is null)
                throw new InvalidOperationException($"Product '{item.ProductId}' referenced by basket was not found.");

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = item.Quantity
            };

            total += orderItem.UnitPrice * orderItem.Quantity;
            order.Items.Add(orderItem);
        }

        order.Total = total;

        await orderRepository.AddAsync(order, cancellationToken);

        basket.Items.Clear();
        await basketRepository.SaveChangesAsync(cancellationToken);

        return order;
    }

    public async Task<Order> PlaceOrderAsync(Guid userId, CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetByUserIdAsync(userId, cancellationToken);
        if (basket is null)
            throw new InvalidOperationException("Basket not found for user.");

        if (basket.Items.Count == 0)
            throw new InvalidOperationException("Basket is empty.");

        var orderItems = new List<OrderItem>(basket.Items.Count);

        foreach (var item in basket.Items)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product is null)
                throw new InvalidOperationException($"Product not found: {item.ProductId}");

            orderItems.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = item.Quantity
            });
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAtUtc = DateTime.UtcNow,
            Items = orderItems
        };

        await orderRepository.AddAsync(order, cancellationToken);

        basket.Items.Clear();
        await basketRepository.SaveChangesAsync(cancellationToken);

        return order;
    }

    public Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken)
        => orderRepository.GetByIdAsync(orderId, cancellationToken);

    public Task<IReadOnlyList<Order>> GetForUserAsync(Guid userId, CancellationToken cancellationToken)
        => orderRepository.GetForUserAsync(userId, cancellationToken);
}
