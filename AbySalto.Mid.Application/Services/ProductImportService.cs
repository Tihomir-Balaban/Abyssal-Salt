using AbySalto.Mid.Application.Contracts;
using AbySalto.Mid.Domain.Entities;

namespace AbySalto.Mid.Application.Services;

public sealed class ProductImportService(
    IExternalProductClient externalClient,
    IProductRepository productRepository)
{
    public async Task<int> ImportAsync(CancellationToken cancellationToken)
    {
        var externalProducts = await externalClient.GetProductsAsync(cancellationToken);

        var mapped = externalProducts
            .Select(x => new Product
            {
                ExternalId = x.ExternalId,
                Name = x.Name,
                Price = x.Price,
                Description = x.Description
            })
            .ToList();

        await productRepository.UpsertByExternalIdAsync(mapped, cancellationToken);

        return mapped.Count;
    }
}