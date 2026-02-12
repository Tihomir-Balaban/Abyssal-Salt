namespace AbySalto.Mid.Application.Contracts.Records;

public sealed record ExternalProduct(
    int ExternalId,
    string Name,
    decimal Price,
    string? Description);