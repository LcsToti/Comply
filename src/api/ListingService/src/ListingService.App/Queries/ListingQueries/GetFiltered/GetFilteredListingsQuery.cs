using ListingService.App.Common;
using ListingService.App.Common.Results;
using MediatR;

namespace ListingService.App.Queries.ListingQueries.GetFiltered;

public record GetFilteredListingsQuery(
    decimal? MinBuyPrice,
    decimal? MaxBuyPrice,
    string? Status,
    Guid? SellerId,
    int Page,
    int PageSize) : IRequest<Result<PagedList<ListingResult>>>;
