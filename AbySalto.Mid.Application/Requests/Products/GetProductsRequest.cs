namespace AbySalto.Mid.Application.Requests.Products;

public sealed class GetProductsRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string SortBy { get; init; } = "name";
    public string SortDirection { get; init; } = "asc";
    public string? Search { get; init; }
}