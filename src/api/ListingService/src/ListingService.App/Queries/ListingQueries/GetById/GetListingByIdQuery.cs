using ListingService.App.Common.Results;
using MediatR;

namespace ListingService.App.Queries.ListingQueries.GetById;

public record GetListingByIdQuery(Guid ListingId) : IRequest<Result<ListingResult>>;