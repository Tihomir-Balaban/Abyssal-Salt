using AbySalto.Mid.Domain.Entities;

namespace AbySalto.Mid.Application.Contracts;


public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid orderId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Order>> GetForUserAsync(Guid userId, CancellationToken cancellationToken);

    Task AddAsync(Order order, CancellationToken cancellationToken);
}