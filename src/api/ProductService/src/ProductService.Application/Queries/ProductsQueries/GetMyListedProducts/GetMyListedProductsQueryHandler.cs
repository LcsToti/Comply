using MediatR;
using MongoDB.Driver;
using ProductService.Application.Responses;
using ProductService.Domain.Entities;

namespace ProductService.Application.Queries.ProductsQueries.GetMyListedProducts;

public class GetMyListedProductsQueryHandler(IMongoDatabase database) : IRequestHandler<GetMyListedProductsQuery, PaginatedList<ProductResponse>>
{
    private readonly IMongoCollection<Product> _products = database.GetCollection<Product>("Products");

    public async Task<PaginatedList<ProductResponse>> Handle(GetMyListedProductsQuery request, CancellationToken cancellationToken)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.SellerId, request.UserId);

        var totalCount = await _products.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        if (totalCount == 0)
            return new PaginatedList<ProductResponse>(request.PageNumber, request.PageSize, 0, 0, []);
        
        var products = await _products
            .Find(filter)
            .SortByDescending(p => p.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Limit(request.PageSize)
            .ToListAsync(cancellationToken);

        var items = products.Select(p => p.ToProductResponse()).ToList();

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return new PaginatedList<ProductResponse>(
            request.PageNumber,
            request.PageSize,
            (int)totalCount,
            totalPages,
            items);
    }
}
