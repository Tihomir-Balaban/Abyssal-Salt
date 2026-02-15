using AbySalto.Mid.Application.Common;
using AbySalto.Mid.Application.Contracts;
using AbySalto.Mid.Application.Products;
using AbySalto.Mid.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AbySalto.Mid.Application.Requests.Products;

public sealed class ProductQueries(IProductRepository repository, IMemoryCache cache)
{
    public Task<PagedResult<ProductDto>> GetPagedAsync(GetProductsRequest request, CancellationToken cancellationToken)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var search = request.Search?.Trim() ?? string.Empty;
        var sortBy = request.SortBy?.Trim() ?? "name";
        var sortDirection = request.SortDirection?.Trim() ?? "asc";

        var cacheKey =
            $"products:p={page}:ps={pageSize}:s={search.ToLowerInvariant()}:sb={sortBy.ToLowerInvariant()}:sd={sortDirection.ToLowerInvariant()}";

        return cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);
            entry.SlidingExpiration = TimeSpan.FromSeconds(30);

            var query = repository.Query();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(q => q.Name.Contains(search));
            }

            query = ApplySorting(query, sortBy, sortDirection);

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
        })!;
    }

    public Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var cacheKey = $"product:{id:D}";

        return cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            entry.SlidingExpiration = TimeSpan.FromMinutes(1);

            var product = await repository.GetByIdAsync(id, cancellationToken);

            if (product is null)
                return null;

            return new ProductDto(
                id: product.Id,
                name: product.Name,
                price: product.Price,
                description: product.Description);
        });
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
