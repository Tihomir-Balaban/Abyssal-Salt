using AbySalto.Mid.Domain.Entities;

namespace AbySalto.Mid.Application.Contracts;

public interface IProductRepository
{
    IQueryable<Product> Query();
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}