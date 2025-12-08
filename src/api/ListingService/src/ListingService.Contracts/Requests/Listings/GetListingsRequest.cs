namespace ListingService.Contracts.Requests.Listings;

public record GetListingsRequest(
    decimal? MinBuyPrice,
    decimal? MaxBuyPrice,
    string? Status,
    Guid? SellerId,
    int Page = 1,
    int PageSize = 20);
