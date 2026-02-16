namespace AbySalto.Mid.Application.Requests.Basket;

public sealed class AddToBasketRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}