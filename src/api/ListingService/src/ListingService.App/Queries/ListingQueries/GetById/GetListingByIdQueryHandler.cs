using ListingService.App.Common.Errors;
using ListingService.App.Common.Results;
using ListingService.App.Common.Results.Mappers;
using ListingService.Domain.ListingAggregate;
using MediatR;
using MongoDB.Driver;

namespace ListingService.App.Queries.ListingQueries.GetById;

public class GetListingByIdQueryHandler(IMongoDatabase database) : IRequestHandler<GetListingByIdQuery, Result<ListingResult>>
{
    private readonly IMongoCollection<Listing> _listingsCollection = database.GetCollection<Listing>("Listings");

    public async Task<Result<ListingResult>> Handle(GetListingByIdQuery request, CancellationToken cancellationToken)
    {
        var filter = Builders<Listing>.Filter.Eq(l => l.Id, request.ListingId);

        var listing = await _listingsCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);

        if (listing is null)
            return Result<ListingResult>.Failure(new NotFound(nameof(Listing), request.ListingId));

        return Result<ListingResult>.Success(listing.ToListingResult());
    }
}

