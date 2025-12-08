using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using ProductService.Application.Constants;
using ProductService.Application.Responses;
using ProductService.Domain.Entities;

namespace ProductService.Application.Queries.ProductsQueries.GetFiltered;

public class GetFilteredProductsQueryHandler(IMongoDatabase database)
    : IRequestHandler<GetFilteredProductsQuery, PaginatedList<ProductResponse>>
{
    private readonly IMongoCollection<Product> _productsCollection = database.GetCollection<Product>("Products");

    private const int MinPageSize = 1;
    private const int MaxPageSize = 30;

    public async Task<PaginatedList<ProductResponse>> Handle(
        GetFilteredProductsQuery request,
        CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
        var pageSize = Math.Clamp(request.PageSize, MinPageSize, MaxPageSize);
        var skip = (pageNumber - 1) * pageSize;

        var filter = BuildFilter(request);

        var totalCount = await _productsCollection
            .CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        var sortDefinition = BuildSort(request.SortBy);

        var findFluent = _productsCollection.Find(filter);

        if (sortDefinition != null)
        {
            findFluent = findFluent.Sort(sortDefinition);
        }

        var entities = await findFluent
            .Skip(skip)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        var items = entities.Select(p => p.ToProductResponse()).ToList();

        return new PaginatedList<ProductResponse>(
            pageNumber,
            pageSize,
            (int)totalCount,
            totalPages,
            items);
    }

    private FilterDefinition<Product> BuildFilter(GetFilteredProductsQuery request)
    {
        var builders = Builders<Product>.Filter;
        var filters = new List<FilterDefinition<Product>>();
        
        filters.Add(builders.Exists(ProductFields.Listing, true));

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var regex = new BsonRegularExpression(request.SearchTerm, "i");  // i -> Case Insensitive
    
            var searchTermFilter = builders.Or(
                builders.Regex(p => p.Title, regex),
                builders.Regex(p => p.Description, regex)
            );
            filters.Add(searchTermFilter);
        }

        if (request.SellerId.HasValue)
            filters.Add(builders.Eq(p => p.SellerId, request.SellerId.Value));

        if (request.Category.HasValue)
            filters.Add(builders.Eq(ProductFields.Category, request.Category.Value.ToString()));


        if (request.Condition.HasValue)
            filters.Add(builders.Eq(ProductFields.Condition, request.Condition.Value.ToString()));

        if (!string.IsNullOrWhiteSpace(request.ListingStatus))
            filters.Add(builders.Eq(ProductFields.ListingStatus, request.ListingStatus));


        if (!string.IsNullOrWhiteSpace(request.AuctionStatus))
            filters.Add(builders.Eq(ProductFields.AuctionStatus, request.AuctionStatus));


        if (request.MinPrice.HasValue)
            filters.Add(builders.Gte(ProductFields.BuyPrice, request.MinPrice.Value));


        if (request.MaxPrice.HasValue)
            filters.Add(builders.Lte(ProductFields.BuyPrice, request.MaxPrice.Value));


        if (request.OnlyAuctionWithoutBids)
        {
            var auctionFilter = builders.Eq(ProductFields.IsAuctionActive, true);
            var noBidsFilter = builders.Size(ProductFields.Bids, 0);

            filters.Add(auctionFilter);
            filters.Add(noBidsFilter);
        }

        if (request.MinBidValue.HasValue)
            filters.Add(builders.Gte(ProductFields.StartBidValue, request.MinBidValue.Value));


        if (request.MaxBidValue.HasValue)
            filters.Add(builders.Lte(ProductFields.StartBidValue, request.MaxBidValue.Value));


        return filters.Count != 0 ? builders.And(filters) : builders.Empty;
    }

    private SortDefinition<Product>? BuildSort(ProductSortBy? sortBy)
    {
        var builders = Builders<Product>.Sort;

        return sortBy switch
        {
            ProductSortBy.Newest => builders.Descending(p => p.CreatedAt),
            ProductSortBy.Oldest => builders.Ascending(p => p.CreatedAt),
            ProductSortBy.PriceAsc => builders.Ascending(ProductFields.BuyPrice),
            ProductSortBy.PriceDesc => builders.Descending(ProductFields.BuyPrice),

            ProductSortBy.MostBids => null,
            ProductSortBy.LessBids => null,
            ProductSortBy.Popularity => builders.Descending(p => p.WatchListCount),
            ProductSortBy.AuctionStartingSoon => builders.Ascending(ProductFields.StartDate),
            ProductSortBy.AuctionEndingSoon => builders.Ascending(ProductFields.EndDate),
            _ => builders.Descending(p => p.CreatedAt) // Default: Newest
        };
    }
}