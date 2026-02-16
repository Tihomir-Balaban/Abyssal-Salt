using AbySalto.Mid.Application.Contracts;
using AbySalto.Mid.Domain.Entities;

namespace AbySalto.Mid.Tests.TestDoubles;

public sealed class InMemoryBasketRepository : IBasketRepository
{
    private readonly Dictionary<Guid, Basket> _byId = new();

    public Task<Basket?> GetByIdAsync(Guid basketId, CancellationToken cancellationToken)
        => Task.FromResult(_byId.GetValueOrDefault(basketId));

    public Task<Basket?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult(_byId.Values.FirstOrDefault(x => x.UserId == userId));

    public Task AddAsync(Basket basket, CancellationToken cancellationToken)
    {
        _byId[basket.Id] = basket;
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}