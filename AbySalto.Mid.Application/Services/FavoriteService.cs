using AbySalto.Mid.Application.Contracts;
using AbySalto.Mid.Domain.Entities;

namespace AbySalto.Mid.Application.Services;

public sealed class FavoriteService(
    IFavoriteRepository favoriteRepository,
    IProductRepository productRepository)
{
    public async Task AddAsync(Guid userId, Guid productId, CancellationToken cancellationToken)
    {
        var exists = await favoriteRepository.ExistsAsync(userId, productId, cancellationToken);
        if (exists)
            throw new InvalidOperationException("Already favorited.");

        var product = await productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
            throw new InvalidOperationException("Product not found.");

        var favorite = new Favorite
        {
            UserId = userId,
            ProductId = productId
        };

        await favoriteRepository.AddAsync(favorite, cancellationToken);
    }

    public async Task RemoveAsync(Guid userId, Guid productId, CancellationToken cancellationToken)
    {
        var exists = await favoriteRepository.ExistsAsync(userId, productId, cancellationToken);
        if (!exists)
            throw new InvalidOperationException("Favorite not found.");

        await favoriteRepository.RemoveAsync(userId, productId, cancellationToken);
    }

    public Task<IReadOnlyList<Product>> GetAsync(Guid userId, CancellationToken cancellationToken)
        => favoriteRepository.GetProductsAsync(userId, cancellationToken);
}