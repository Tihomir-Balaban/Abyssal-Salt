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
}