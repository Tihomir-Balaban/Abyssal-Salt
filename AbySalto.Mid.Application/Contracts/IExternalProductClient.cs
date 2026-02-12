namespace AbySalto.Mid.Application.Contracts;

public interface IExternalProductClient
{
    Task<IReadOnlyList<ExternalProduct>> GetProductsAsync(CancellationToken cancellationToken);
}