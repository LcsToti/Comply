namespace ProductService.Application.Constants;

public static class ProductFields
{
    // Product
    public const string Id = "ProductId";
    public const string Title = "Title";
    public const string Description = "Description";
    public const string SellerId = "SellerId";
    public const string Category = "Category";
    public const string Condition = "Condition";
    public const string WatchListCount = "WatchListCount";
    public const string CreatedAt = "CreatedAt";

    // Listing
    public const string Listing = "Listing";
    public const string ListingStatus = "Listing.Status";
    public const string ListedAt = "Listing.ListedAt";
    public const string BuyPrice = "Listing.BuyPrice";
    public const string IsAuctionActive = "Listing.IsAuctionActive";

    // Auction
    public const string Auction = "Listing.Auction";
    public const string AuctionStatus = "Listing.Auction.Status";
    public const string Bids = "Listing.Auction.Bids";

    // Auction Settings
    public const string StartBidValue = "Listing.Auction.Settings.StartBidValue";
    public const string EndDate = "Listing.Auction.Settings.EndDate";
    public const string StartDate = "Listing.Auction.Settings.StartDate";

    // Not used
    public const string WinBidValue = "Listing.Auction.Settings.WinBidValue";
}