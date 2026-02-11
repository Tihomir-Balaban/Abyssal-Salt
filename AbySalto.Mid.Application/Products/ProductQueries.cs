using AbySalto.Mid.Application.Common;
using AbySalto.Mid.Application.Contracts;
using AbySalto.Mid.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.Mid.Application.Products;

public sealed class ProductQueries(IProductRepository repository)
{
    public async Task<PagedResult<ProductDto>> GetPagedAsync(GetProductsRequest request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 10 : request.PageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        var query = repository.Query();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(q => q.Name.Contains(search));
        }

        query = ApplySorting(query, request.SortBy, request.SortDirection);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(product => new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<ProductDto>(
            page: page,
            pageSize: pageSize,
            totalCount: totalCount,
            totalPages: totalPages,
            items: items);
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);

        if (product is null)
            return null;

        return new ProductDto(
            id: product.Id,
            name: product.Name,
            price: product.Price,
            description: product.Description);
    }

    private static IQueryable<Product> ApplySorting(
        IQueryable<Product> query,
        string sortBy,
        string sortDirection
    )
    {
        var direction = sortDirection?.Trim().ToLowerInvariant();
        var description = direction == "desc";

        var key = sortBy?.Trim().ToLowerInvariant();

        return key switch
        {
            "price" => description ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price),
            "name" or _ => description ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name)
        };
    }
}
