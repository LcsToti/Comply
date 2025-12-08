using ListingService.App.Common.Results;
using MediatR;

namespace ListingService.App.Queries.AuctionQueries.GetById;

public record GetAuctionByIdQuery(Guid AuctionId) : IRequest<Result<AuctionResult>>;
