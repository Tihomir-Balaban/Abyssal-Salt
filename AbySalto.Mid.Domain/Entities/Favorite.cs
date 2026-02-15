namespace AbySalto.Mid.Domain.Entities;

public sealed class Favorite
{
    public Guid Id { get; set; }
    public Guid UserId { get; init; }
    public Guid ProductId { get; init; }
    public Product Product { get; set; } = null!;

    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
}