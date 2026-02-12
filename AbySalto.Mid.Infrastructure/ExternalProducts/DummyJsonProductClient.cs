using System.Net.Http.Json;
using AbySalto.Mid.Application.Contracts;

namespace AbySalto.Mid.Infrastructure.ExternalProducts;

public sealed class DummyJsonProductClient(HttpClient httpClient) : IExternalProductClient
{
    public async Task<IReadOnlyList<ExternalProduct>> GetProductsAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetFromJsonAsync<DummyJsonProductsResponse>(
            "products?limit=100",
            cancellationToken);

        var items = response?.Products ?? [];

        return items
            .Select(x => new ExternalProduct(
                x.Id,
                x.Title,
                x.Price,
                x.Description))
            .ToList();
    }

    private sealed record DummyJsonProductsResponse(List<DummyJsonProduct> Products);

    private sealed record DummyJsonProduct(
        int Id,
        string Title,
        decimal Price,
        string? Description);
}
