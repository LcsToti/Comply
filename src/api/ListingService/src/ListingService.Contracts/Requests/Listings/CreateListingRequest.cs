namespace ListingService.Contracts.Requests.Listings;

public record CreateListingRequest(
    Guid ProductId,
    decimal BuyPrice);