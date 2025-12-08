using ListingService.Domain.Common;
using ListingService.Domain.Exceptions;

namespace ListingService.Domain.ListingAggregate;

public class Listing : AggregateRoot 
{
    public Guid SellerId { get; set; }
    public Guid ProductId { get; private set; }
    public ListingStatus Status { get; private set; }
    public decimal BuyPrice { get; private set; }
    public bool IsAuctionActive { get; private set; }
    public bool IsProcessingPurchase { get; private set; }
    public Guid? BuyerId { get; private set; }
    public Guid? AuctionId { get; private set; }
    public DateTime ListedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Listing(Guid sellerId, Guid productId, decimal buyPrice, DateTime listedAt, DateTime updatedAt) : base(Guid.NewGuid())
    {
        SellerId = sellerId;
        ProductId = productId;
        BuyPrice = buyPrice;
        IsAuctionActive = false;
        IsProcessingPurchase = false;
        Status = ListingStatus.Available;

        ListedAt = listedAt;
        UpdatedAt = updatedAt;
    }

    #region For ORM
    private Listing() { }

    internal static Listing Restore(
        Guid id,
        Guid sellerId,
        Guid productId,
        ListingStatus status,
        decimal buyPrice,
        bool isAuctionActive,
        bool isProcessingPurchase,
        Guid? buyerId,
        Guid? auctionId,
        DateTime listedAt,
        DateTime updatedAt
        )
    {
        return new Listing
        {
            Id = id,
            SellerId = sellerId,
            ProductId = productId,
            Status = status,
            BuyPrice = buyPrice,
            IsAuctionActive = isAuctionActive,
            IsProcessingPurchase = isProcessingPurchase,
            BuyerId = buyerId,
            AuctionId = auctionId,
            ListedAt = listedAt,
            UpdatedAt = updatedAt
        };
    }
    #endregion

    /// <summary> Creates a new listing with the given data. </summary>
    /// <exception cref="InvalidListingException"> Thrown if the buy price is not within the valid range. </exception>
    public static Listing Create(Guid sellerId, Guid productId, decimal buyPrice, DateTime createdAt)
    {
        if (buyPrice < DomainConstants.MinTransactionValue) 
            throw new InvalidListingException($"Buy price should be at least {DomainConstants.MinTransactionValue:C}.");

        if (buyPrice > DomainConstants.MaxTransactionValue) 
            throw new InvalidListingException($"Buy price should not exceed {DomainConstants.MaxTransactionValue:C}");

        var listedAt = createdAt;
        var updatedAt = listedAt;

        var listing = new Listing(sellerId, productId, buyPrice, listedAt, updatedAt);

        return listing;
    }

    #region Public Methods
    /// <summary>Updates the buy price of the listing.</summary>
    /// <exception cref="InvalidListingException">Thrown if the new price is invalid or if the product is sold or in an auction.</exception>
    public void UpdateBuyPrice(decimal newBuyPrice, DateTime updatedAt)
    {
        if (newBuyPrice <= DomainConstants.MinTransactionValue)
            throw new InvalidListingException($"New buy price should be at least {DomainConstants.MinTransactionValue:C}.");

        if (newBuyPrice > DomainConstants.MaxTransactionValue)
            throw new InvalidListingException($"New buy price should not exceed {DomainConstants.MaxTransactionValue:C}");

        if (Status != ListingStatus.Available && Status != ListingStatus.Paused)
            throw new InvalidListingException("It is only possible to update a buy price of a listing if its Available or Paused.");
        if (IsAuctionActive)
            throw new InvalidListingException("It is not possible to change the buy price during an auction.");

        if (IsProcessingPurchase)
            throw new InvalidAuctionException("It is not possible to update this product because a buy attempt is being processed");

        BuyPrice = newBuyPrice;
        MarkAsUpdated(updatedAt);
    }

    /// <summary>Toggles the availability status of a listing.</summary>
    /// <exception cref="InvalidListingException">Thrown if the product is sold or currently in an auction.</exception>
    public void ToggleAvailability(DateTime updatedAt)
    {
        if (Status != ListingStatus.Available && Status != ListingStatus.Paused)
            throw new InvalidListingException("It is only possible to change the visibility of a listing if its Available or Paused.");
        if (AuctionId.HasValue)
            throw new InvalidListingException("It is not possible to change the visibility of a listing with an auction.");

        if (IsProcessingPurchase) 
            throw new InvalidAuctionException("It's not possible to toggle the availability of this product because a buy attempt is being processed");

        Status = Status.ToggleVisibilityStatuses();
        MarkAsUpdated(updatedAt);
    }

    /// <summary> Validates whether the product is eligible for a Buy Now operation. </summary>
    /// <exception cref="InvalidListingException">Thrown if the product is not available or the Buy Now option is locked. </exception>
    public void PrepareBuyNow()
    {
        if (Status != ListingStatus.Available) throw new InvalidListingException("Product is not available for sold.");
        if (IsAuctionActive) throw new InvalidListingException("Buy Now option is locked during an auction.");
    }

    public void AbortBuyNow()
    {
        if (!IsProcessingPurchase)
            throw new Exception("It is not possible to abort the purchase of a product that is not being processed.");
        IsProcessingPurchase = false;
    }

    public void CompleteBuyNow(Guid buyerId, DateTime BoughtAt)
    {
        if (!IsProcessingPurchase) 
            throw new Exception("It is not possible to complete the purchase of a product that is not being processed.");

        IsProcessingPurchase = false;
        Status = ListingStatus.Sold;
        BuyerId = buyerId;

        MarkAsUpdated(BoughtAt);
    }
    #endregion

    #region Event Reacters
    /// <summary>Marks the listing as sold via auction to the specified winner.</summary>
    /// <exception cref="InvalidListingException">Thrown if the listing is not attached to an auction or is already sold.</exception>
    public void MarkAsSoldByAuction(Guid winnerId, DateTime SoldAt)
    {
        if (AuctionId.HasValue == false)
            throw new InvalidListingException("It is not possible to sell a product via auction if it is not attached to an auction.");

        if (Status == ListingStatus.Sold || BuyerId.HasValue == true)
            throw new InvalidListingException("It is not possible to sell a product that is already sold.");

        Status = ListingStatus.SoldByAuction;
        BuyerId = winnerId;
        MarkAsUpdated(SoldAt);
    }

    /// <summary>Attaches an auction to the listing.</summary>
    /// <exception cref="InvalidListingException">Thrown if the product is sold, already in auction, or already attached to an auction.</exception>
    public void AttachAuction(Guid auctionId, DateTime updatedAt)
    {
        if (Status == ListingStatus.Sold || Status == ListingStatus.SoldByAuction)
            throw new InvalidListingException("It is not possible to attach an auction to a sold product.");
        if (IsAuctionActive || AuctionId.HasValue)
            throw new InvalidListingException("It is not possible to attach an auction to a product that is already in auction.");
       
        AuctionId = auctionId;
        MarkAsUpdated(updatedAt);
    }

    /// <summary>Detaches the auction from the listing.</summary>
    /// <exception cref="InvalidListingException">Thrown if the listing is not attached to any auction, or if it is sold or currently in an auction.</exception>
    public void DetachAuction(DateTime updatedAt)
    {
        if (AuctionId.HasValue == false)
            throw new InvalidListingException("This product is not attached to any auction.");
        if (Status == ListingStatus.SoldByAuction)
            throw new InvalidListingException("It is not possible to detach an auction from product sold by an auction.");
        if (IsAuctionActive)
            throw new InvalidListingException("It is not possible to detach an auction from a product during an active auction.");

        AuctionId = null;
        MarkAsUpdated(updatedAt);
    }
    
    /// <summary>Marks the product as in auction, validating that it is available and attached to an auction.</summary>
    /// <exception cref="InvalidListingException">Thrown if the product is not available or not attached to an auction.</exception>
    public void SetAuctionActive(DateTime updatedAt)
    {
        if (Status != ListingStatus.Available)
            throw new InvalidListingException("It is only possible to mark as in auction a product that is available.");

        if (AuctionId.HasValue == false)
            throw new InvalidListingException("It is not possible to mark as in auction a product that is not attached to an auction.");

        IsAuctionActive = true;
        MarkAsUpdated(updatedAt);
    }

    /// <summary>Marks the product as available, only if it is currently in auction.</summary>
    /// <exception cref="InvalidListingException">Thrown if the product is not currently in auction.</exception>
    public void SetAuctionInactive(DateTime updatedAt)
    {
        if (!IsAuctionActive)
            throw new InvalidListingException("It is only possible to mark as available a product that is in auction.");

        IsAuctionActive = false;
        MarkAsUpdated(updatedAt);
    }
    #endregion

    private void MarkAsUpdated(DateTime updatedAt)
    {
        UpdatedAt = updatedAt;
    }
}