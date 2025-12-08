using ListingService.App.Common.Results;
using MediatR;

namespace ListingService.App.Queries.AuctionQueries.GetAuctionBids;

public record GetAuctionBidsQuery(Guid AuctionId) : IRequest<Result<List<BidResult>>>;
