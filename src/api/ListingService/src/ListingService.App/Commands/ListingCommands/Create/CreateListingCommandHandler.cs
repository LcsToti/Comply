using ListingService.App.Common;
using ListingService.App.Common.Errors;
using ListingService.App.Common.Interfaces;
using ListingService.App.Common.Results;
using ListingService.App.Common.Results.Mappers;
using ListingService.Domain.ListingAggregate;
using ListingService.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ListingService.App.Commands.ListingCommands.Create;

public class CreateListingCommandHandler(
        IListingRepository listingRepository,
        RepositoryCommandsOrchestrator repositoryCommandsOrchestrator,
        IProductServiceClient productClient,
        IDateTimeProvider dateTimeProvider,
        ILogger<CreateListingCommandHandler> logger) : IRequestHandler<CreateListingCommand, Result<ListingResult>>
{
    private readonly IListingRepository _listingRepository = listingRepository;
    private readonly RepositoryCommandsOrchestrator _repositoryCommandsOrchestrator = repositoryCommandsOrchestrator;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ILogger<CreateListingCommandHandler> _logger = logger;
    private readonly IProductServiceClient _productClient = productClient;

    public async Task<Result<ListingResult>> Handle(CreateListingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateListingCommand for Product {ProductId}.", request.ProductId);

        // App Logic
        var sellerId = await _productClient.GetSellerIdByProductIdAsync(request.ProductId, cancellationToken);
        if (sellerId is null)
            return Result<ListingResult>.Failure(new NotFound("Product", request.ProductId));
        if (sellerId != request.UserId)
            return Result<ListingResult>.Failure(new Forbidden("Users can only list their own products."));

        var checkListing = await _listingRepository.GetByProductIdAsync(request.ProductId);

        if (checkListing != null) 
            return Result<ListingResult>.Failure(new Conflict("A product can only be listed once."));

        // Domain
        var listing = Listing.Create(request.UserId, request.ProductId, request.BuyPrice, _dateTimeProvider.UtcNow);

        // Persist
        await _repositoryCommandsOrchestrator.AddListingAsync(listing, cancellationToken);

        // Finish
        _logger.LogInformation("Listing {Id} created successfully", listing.Id);
        return Result<ListingResult>.Success(listing.ToListingResult());
    }
}
