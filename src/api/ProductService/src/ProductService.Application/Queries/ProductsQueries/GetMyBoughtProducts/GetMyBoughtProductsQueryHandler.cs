using MediatR;
using MongoDB.Driver;
using ProductService.Application.Queries.ProductsQueries.GetMyListedProducts;
using ProductService.Application.Responses;
using ProductService.Domain.Entities;

namespace ProductService.Application.Queries.ProductsQueries.GetMyBoughtProducts;

public class GetMyBoughtProductsQueryHandler(IMongoDatabase database) : IRequestHandler<GetMyBoughtProductsQuery, PaginatedList<ProductResponse>>
{
    private readonly IMongoCollection<Product> _products = database.GetCollection<Product>("Products");

    public async Task<PaginatedList<ProductResponse>> Handle(GetMyBoughtProductsQuery request, CancellationToken cancellationToken)
    {
        var filter = Builders<Product>.Filter.Or(
            Builders<Product>.Filter.Eq(p => p.Listing!.BuyerId, request.UserId),
            Builders<Product>.Filter.ElemMatch(
                p => p.Listing!.Auction!.Bids,
                b => b.BidderId == request.UserId && b.Status == "Winner"));

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
