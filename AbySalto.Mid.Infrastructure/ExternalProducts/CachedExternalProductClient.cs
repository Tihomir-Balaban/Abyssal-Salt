using AbySalto.Mid.Application.Contracts;
using AbySalto.Mid.Application.Contracts.Records;
using Microsoft.Extensions.Caching.Memory;

namespace AbySalto.Mid.Infrastructure.ExternalProducts;

public sealed class CachedExternalProductClient(
    DummyJsonProductClient inner,
    IMemoryCache cache) : IExternalProductClient
{
    private const string CacheKey = "external-products:v1";
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(10);

    public async Task<IReadOnlyList<ExternalProduct>> GetProductsAsync(CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(CacheKey, out IReadOnlyList<ExternalProduct>? cached) && cached is not null)
            return cached;

        var fresh = await inner.GetProductsAsync(cancellationToken);

        cache.Set(CacheKey, fresh, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = Ttl
        });

        return fresh;
    }
}