namespace ListingService.Contracts.Requests.Auctions;

public record EditAuctionSettingsRequest(
    decimal? StartBidValue,
    decimal? WinBidValue,
    DateTime? StartDate,
    DateTime? EndDate);
