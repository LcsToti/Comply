using ListingService.App.Common;
using ListingService.App.Common.Results;
using MediatR;

namespace ListingService.App.Queries.AuctionQueries.GetFiltered;

public record GetFilteredAuctionsQuery(
    decimal? MaxStartBid,
    DateTime? StartsBefore,
    DateTime? EndsBefore,
    int Page,
    int PageSize) : IRequest<Result<PagedList<AuctionResult>>>;

