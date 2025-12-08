using ListingService.App.Common.Errors;
using ListingService.App.Common.Results;
using ListingService.App.Common.Results.Mappers;
using ListingService.Domain.AuctionAggregate.Entities;
using MediatR;
using MongoDB.Driver;

namespace ListingService.App.Queries.AuctionQueries.GetAuctionBids;

public class GetAuctionBidsQueryHandler(IMongoDatabase database)
    : IRequestHandler<GetAuctionBidsQuery, Result<List<BidResult>>>
{
    private readonly IMongoCollection<Auction> _auctionsCollection = database.GetCollection<Auction>("Auctions");

    public async Task<Result<List<BidResult>>> Handle(GetAuctionBidsQuery request, CancellationToken cancellationToken)
    {
        var filter = Builders<Auction>.Filter.Eq(a => a.Id, request.AuctionId);

        var auction = await _auctionsCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);

        if (auction is null)
            return Result<List<BidResult>>.Failure(new NotFound(nameof(Auction), request.AuctionId));

        var bidResults = auction.Bids.Select(bid => bid.ToBidResult()).ToList();

        return Result<List<BidResult>>.Success(bidResults);
    }
}
