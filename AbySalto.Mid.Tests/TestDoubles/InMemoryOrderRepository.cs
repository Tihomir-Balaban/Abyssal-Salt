using AbySalto.Mid.Application.Contracts;
using AbySalto.Mid.Domain.Entities;

namespace AbySalto.Mid.Tests.TestDoubles;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<Guid, Order> _byId = new();

    public Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken)
        => Task.FromResult(_byId.GetValueOrDefault(orderId));

    public Task<IReadOnlyList<Order>> GetForUserAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<Order>>(_byId.Values.Where(x => x.UserId == userId).ToList());

    public Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        _byId[order.Id] = order;
        return Task.CompletedTask;
    }
}