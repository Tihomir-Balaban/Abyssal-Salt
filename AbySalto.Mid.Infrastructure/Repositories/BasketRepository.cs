using AbySalto.Mid.Application.Contracts;
using AbySalto.Mid.Domain.Entities;
using AbySalto.Mid.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using AbySalto.Mid.Application.Contracts;
using AbySalto.Mid.Domain.Entities;
using AbySalto.Mid.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.Mid.Infrastructure.Repositories;

public sealed class BasketRepository(AppDbContext db) : IBasketRepository
{
    public Task<Basket?> GetByIdAsync(Guid basketId, CancellationToken cancellationToken)
        => db.Baskets
            .Include(b => b.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(b => b.Id == basketId, cancellationToken);

    public Task AddAsync(Basket basket, CancellationToken cancellationToken)
        => db.Baskets.AddAsync(basket, cancellationToken).AsTask();

    public Task SaveChangesAsync(CancellationToken cancellationToken)
        => db.SaveChangesAsync(cancellationToken);
}
