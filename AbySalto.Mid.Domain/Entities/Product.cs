namespace AbySalto.Mid.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string? Description { get; set; }
}