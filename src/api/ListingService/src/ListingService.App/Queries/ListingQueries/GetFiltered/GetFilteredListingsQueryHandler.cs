using ListingService.App.Common;
using ListingService.App.Common.Results;
using ListingService.App.Common.Results.Mappers;
using ListingService.Domain.ListingAggregate;
using MediatR;
using MongoDB.Driver;

namespace ListingService.App.Queries.ListingQueries.GetFiltered;

public class GetFilteredListingsQueryHandler(IMongoDatabase database) : IRequestHandler<GetFilteredListingsQuery, Result<PagedList<ListingResult>>>
{
    private readonly IMongoCollection<Listing> _listingsCollection = database.GetCollection<Listing>("Listings");
    private const int MaxPageSize = 100;

    public async Task<Result<PagedList<ListingResult>>> Handle(GetFilteredListingsQuery request, CancellationToken cancellationToken)
    {
        var pageSize = request.PageSize > MaxPageSize ? MaxPageSize : request.PageSize;

        var filterBuilder = Builders<Listing>.Filter;
        var filter = filterBuilder.Empty;

        if (request.MinBuyPrice.HasValue)
        {
            filter &= filterBuilder.Gte(l => l.BuyPrice, request.MinBuyPrice.Value);
        }

        if (request.MaxBuyPrice.HasValue)
        {
            filter &= filterBuilder.Lte(l => l.BuyPrice, request.MaxBuyPrice.Value);
        }

        if (request.SellerId.HasValue)
        {
            filter &= filterBuilder.Eq(l => l.SellerId, request.SellerId.Value);
        }

        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<ListingStatus>(request.Status, true, out var statusEnum))
        {
            filter &= filterBuilder.Eq(l => l.Status, statusEnum);
        }

        long totalCount = await _listingsCollection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var listings = await _listingsCollection.Find(filter)
            .Skip((request.Page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        var listingResults = listings.Select(l => l.ToListingResult()).ToList();

        var pagedList = new PagedList<ListingResult>(
            listingResults,
            request.Page,
            pageSize,
            (int)totalCount);

        return Result<PagedList<ListingResult>>.Success(pagedList);
    }
}