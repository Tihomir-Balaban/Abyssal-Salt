using AbySalto.Mid.Domain.Entities;

namespace AbySalto.Mid.Application.Contracts;

public interface IBasketRepository
{
    Task<Domain.Entities.Basket?> GetByIdAsync(Guid basketId, CancellationToken cancellationToken);
    Task<Domain.Entities.Basket> CreateAsync(CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}