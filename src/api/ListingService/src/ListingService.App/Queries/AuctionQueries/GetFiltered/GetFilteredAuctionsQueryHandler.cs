using ListingService.App.Common;
using ListingService.App.Common.Results;
using ListingService.App.Common.Results.Mappers;
using ListingService.Domain.AuctionAggregate.Entities;
using MediatR;
using MongoDB.Driver;

namespace ListingService.App.Queries.AuctionQueries.GetFiltered;

public class GetFilteredAuctionsQueryHandler(IMongoDatabase database) : IRequestHandler<GetFilteredAuctionsQuery, Result<PagedList<AuctionResult>>>
{
    private readonly IMongoCollection<Auction> _auctionsCollection = database.GetCollection<Auction>("Auctions");

    public async Task<Result<PagedList<AuctionResult>>> Handle(GetFilteredAuctionsQuery request, CancellationToken cancellationToken)
    {
        var pageSize = request.PageSize > AppConstants.MaxPageSize ? AppConstants.MaxPageSize : request.PageSize;

        var filterBuilder = Builders<Auction>.Filter;
        var filter = filterBuilder.Empty;

        if (request.MaxStartBid.HasValue)
        {
            var priceFilter = filterBuilder.Lte(a => a.Settings.StartBidValue, request.MaxStartBid.Value);
            filter &= priceFilter;
        }

        if (request.StartsBefore.HasValue)
        {
            var dateFilter = filterBuilder.Lte(a => a.Settings.StartDate, request.StartsBefore.Value);
            filter &= dateFilter;
        }

        if (request.EndsBefore.HasValue)
        {
            var dateFilter = filterBuilder.Lte(a => a.Settings.EndDate, request.EndsBefore.Value);
            filter &= dateFilter;
        }

        long totalCount = await _auctionsCollection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var auctions = await _auctionsCollection.Find(filter)
            .Skip((request.Page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        var auctionResults = auctions.Select(a => a.ToAuctionResult()).ToList();

        var pagedList = new PagedList<AuctionResult>(
            auctionResults,
            request.Page,
            pageSize,
            (int)totalCount);

        return Result<PagedList<AuctionResult>>.Success(pagedList);
    }
}

