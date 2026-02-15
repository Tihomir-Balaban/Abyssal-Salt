namespace AbySalto.Mid.Domain.Entities;

public sealed class Favorite
{
    public Guid UserId { get; init; }
    public Guid ProductId { get; init; }

    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
}