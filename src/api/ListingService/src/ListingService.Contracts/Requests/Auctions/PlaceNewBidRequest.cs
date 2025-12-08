namespace ListingService.Contracts.Requests.Auctions;

public record PlaceNewBidRequest(
    decimal Value,
    string PaymentMethodId);
