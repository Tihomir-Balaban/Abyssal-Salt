namespace AbySalto.Mid.Domain.Entities;

public sealed class BasketItem
{
    public Guid Id { get; set; }
    public Guid BasketId { get; set; }
    public Product Product { get; set; } = null!;
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}