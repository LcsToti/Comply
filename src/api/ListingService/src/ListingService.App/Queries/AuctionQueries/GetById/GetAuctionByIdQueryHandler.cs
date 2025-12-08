using ListingService.App.Common.Errors;
using ListingService.App.Common.Results;
using ListingService.App.Common.Results.Mappers;
using ListingService.Domain.AuctionAggregate.Entities;
using MediatR;
using MongoDB.Driver;

namespace ListingService.App.Queries.AuctionQueries.GetById;

public class GetAuctionByIdQueryHandler(IMongoDatabase database) : IRequestHandler<GetAuctionByIdQuery, Result<AuctionResult>>
{
    private readonly IMongoCollection<Auction> _auctionsCollection = database.GetCollection<Auction>("Auctions");

    public async Task<Result<AuctionResult>> Handle(GetAuctionByIdQuery request, CancellationToken cancellationToken)
    {
        var filter = Builders<Auction>.Filter.Eq(a => a.Id, request.AuctionId);

        var auction = await _auctionsCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);

        if (auction is null)
            return Result<AuctionResult>.Failure(new NotFound(nameof(Auction), request.AuctionId));
        
        return Result<AuctionResult>.Success(auction.ToAuctionResult());
    }
}