namespace AbySalto.Mid.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; set; }

    // Nullable to allow guest checkout if you decide to support it.
    public Guid? UserId { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public decimal Total { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}