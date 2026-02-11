namespace AbySalto.Mid.Application.Products;

public sealed class ProductDto
{
    public ProductDto()
    {
    }

    public ProductDto(Guid id, string name, decimal price, string? description)
    {
        Id = id;
        Name = name;
        Price = price;
        Description = description;
    }

    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string? Description { get; init; }
}
