using AbySalto.Mid.Application.Contracts;
using AbySalto.Mid.Domain.Entities;
using AbySalto.Mid.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.Mid.Infrastructure.Repositories;

public sealed class FavoriteRepository(AppDbContext db) : IFavoriteRepository
{
    public Task<bool> ExistsAsync(Guid userId, Guid productId, CancellationToken cancellationToken)
    {
        return db.Favorites.AnyAsync(
            x => x.UserId == userId && x.ProductId == productId,
            cancellationToken);
    }

    public async Task AddAsync(Favorite favorite, CancellationToken cancellationToken)
    {
        db.Favorites.Add(favorite);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(Guid userId, Guid productId, CancellationToken cancellationToken)
    {
        var entity = await db.Favorites
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId, cancellationToken);

        if (entity is null)
            return;

        db.Favorites.Remove(entity);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await db.Favorites
            .Where(f => f.UserId == userId)
            .Join(db.Products, f => f.ProductId, p => p.Id, (_, p) => p)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}