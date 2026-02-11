namespace AbySalto.Mid.Domain.Entities;

public sealed class BasketItem
{
    public Guid Id { get; set; }

    public Guid BasketId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }
}