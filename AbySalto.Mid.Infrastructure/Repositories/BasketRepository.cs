using AbySalto.Mid.Application.Contracts;
using AbySalto.Mid.Domain.Entities;
using AbySalto.Mid.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.Mid.Infrastructure.Repositories;

public sealed class BasketRepository(AppDbContext db) : IBasketRepository
{
    public async Task<Basket?> GetByIdAsync(Guid basketId, CancellationToken cancellationToken)
        => await db.Baskets
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == basketId, cancellationToken);

    public async Task<Basket> CreateAsync(CancellationToken cancellationToken)
    {
        var basket = new Basket
        {
            Id = Guid.NewGuid(),
            CreatedAtUtc = DateTime.UtcNow
        };

        db.Baskets.Add(basket);
        await db.SaveChangesAsync(cancellationToken);

        return basket;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => db.SaveChangesAsync(cancellationToken);
}