using AbySalto.Mid.Application.Contracts;
using AbySalto.Mid.Domain.Entities;

namespace AbySalto.Mid.Tests.TestDoubles;

public sealed class InMemoryProductRepository : IProductRepository
{
    private readonly Dictionary<Guid, Product> _byId = new();

    public IQueryable<Product> Query() => _byId.Values.AsQueryable();

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => Task.FromResult(_byId.GetValueOrDefault(id));

    public Task UpsertByExternalIdAsync(IReadOnlyList<Product> products, CancellationToken cancellationToken)
    {
        foreach (var product in products)
            _byId[product.Id] = product;

        return Task.CompletedTask;
    }

    public void Seed(params Product[] products)
    {
        foreach (var product in products)
            _byId[product.Id] = product;
    }
}