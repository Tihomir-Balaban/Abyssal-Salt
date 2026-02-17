using AbySalto.Mid.Application.Common.Exceptions;
using AbySalto.Mid.Application.Contracts;
using AbySalto.Mid.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace AbySalto.Mid.Application.Services;

public sealed class FavoriteService(
    IFavoriteRepository favoriteRepository,
    IProductRepository productRepository)
{
    public async Task AddAsync(Guid userId, Guid productId, CancellationToken cancellationToken)
    {
        var exists = await favoriteRepository.ExistsAsync(userId, productId, cancellationToken);
        if (exists)
            throw new AppException("Already favorited.", StatusCodes.Status409Conflict, "favorite_exists");

        var product = await productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
            throw new AppException("Product not found.", StatusCodes.Status404NotFound, "product_not_found");


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
            throw new AppException("Favorite not found.", StatusCodes.Status404NotFound, "favorite_not_found");


        await favoriteRepository.RemoveAsync(userId, productId, cancellationToken);
    }

    public Task<IReadOnlyList<Product>> GetAsync(Guid userId, CancellationToken cancellationToken)
        => favoriteRepository.GetProductsAsync(userId, cancellationToken);
}