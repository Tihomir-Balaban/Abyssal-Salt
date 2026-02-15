using AbySalto.Mid.Domain.Entities;

namespace AbySalto.Mid.Application.Contracts;

public interface IFavoriteRepository
{
    Task<bool> ExistsAsync(Guid userId, Guid productId, CancellationToken cancellationToken);

    Task AddAsync(Favorite favorite, CancellationToken cancellationToken);

    Task RemoveAsync(Guid userId, Guid productId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Product>> GetProductsAsync(Guid userId, CancellationToken cancellationToken);
}