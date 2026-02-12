using AbySalto.Mid.Application.Contracts;
using AbySalto.Mid.Domain.Entities;
using AbySalto.Mid.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.Mid.Infrastructure.Repositories;

public sealed class ProductRepository(AppDbContext db) : IProductRepository
{
    public IQueryable<Product> Query()
        => db.Products.AsNoTracking();

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    
    public async Task UpsertByExternalIdAsync(
        IReadOnlyList<Product> products,
        CancellationToken cancellationToken)
    {
        var externalIds = products
            .Where(x => x.ExternalId.HasValue)
            .Select(x => x.ExternalId!.Value)
            .ToList();

        var existing = await db.Products
            .Where(x => x.ExternalId.HasValue && externalIds.Contains(x.ExternalId.Value))
            .ToListAsync(cancellationToken);

        var existingByExternalId = existing
            .Where(x => x.ExternalId.HasValue)
            .ToDictionary(x => x.ExternalId!.Value, x => x);

        foreach (var incoming in products)
        {
            if (!incoming.ExternalId.HasValue)
                continue;

            if (existingByExternalId.TryGetValue(incoming.ExternalId.Value, out var current))
            {
                current.Name = incoming.Name;
                current.Price = incoming.Price;
                current.Description = incoming.Description;
            }
            else
            {
                db.Products.Add(incoming);
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}