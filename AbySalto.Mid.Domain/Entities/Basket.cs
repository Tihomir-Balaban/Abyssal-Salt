namespace AbySalto.Mid.Domain.Entities;

public sealed class Basket
{
    public Guid Id { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<BasketItem> Items { get; set; } = new();
}