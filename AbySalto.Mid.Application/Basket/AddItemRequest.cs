namespace AbySalto.Mid.Application.Basket;

public sealed class AddItemRequest
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}