using ListingService.Domain.AuctionAggregate.Enums;
using ListingService.Domain.AuctionAggregate.ValueObjects;
using ListingService.Domain.Common;
using ListingService.Domain.Exceptions;

namespace ListingService.Domain.AuctionAggregate.Entities;

public class Auction : AggregateRoot
{
    private readonly List<Bid> _bids = [];

    public Guid ListingId { get; private set; }
    public IReadOnlyCollection<Bid> Bids => _bids.AsReadOnly();
    public AuctionStatus Status { get; private set; }
    public AuctionSettings Settings { get; private set; } = null!;
    public int Version { get; private set; }
    public bool IsProcessingBid { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }

    private Auction(Guid listingId, AuctionSettings settings) : base(Guid.NewGuid())
    {
        ListingId = listingId;
        Settings = settings;
        Version = 1;
        Status = AuctionStatus.Awaiting;
        IsProcessingBid = false;
    }

    #region For ORM
    private Auction() { }

    internal static Auction Restore(
        Guid id,
        Guid listingId,
        AuctionSettings settings,
        AuctionStatus status,
        int version,
        bool isProcessingBid,
        DateTime? editedAt,
        DateTime? startedAt,
        DateTime? endedAt,
        List<Bid> bids)
    {
        var auction = new Auction
        {
            Id = id,
            ListingId = listingId,
            Settings = settings,
            Status = status,
            Version = version,
            IsProcessingBid = isProcessingBid,
            EditedAt = editedAt,
            StartedAt = startedAt,
            EndedAt = endedAt
        };

        auction._bids.AddRange(bids);

        return auction;
    }
    #endregion

    public static Auction Create(Guid listingId, AuctionSettings settings)
    {
        var auction = new Auction(listingId, settings);
        return auction;
    }

    /// <summary>Automatically starts the auction.</summary>
    /// <exception cref="InvalidAuctionException">Thrown if the auction is not in Awaiting status.</exception>
    public void Start(DateTime utcNow)
    {
        if (Status != AuctionStatus.Awaiting) throw new InvalidAuctionException("Its only possible to start an awaiting auction.");

        Status = AuctionStatus.Active;
        StartedAt = utcNow;
        Version++;
    }

    /// <summary>Automatically sets the auction as ending.</summary>
    /// <exception cref="InvalidAuctionException">Thrown if the auction is not in active status.</exception>
    public void SetAsEnding()
    {
        if (Status != AuctionStatus.Active) throw new InvalidAuctionException("Only an active auction can be set as ending.");

        Status = AuctionStatus.Ending;
        Version++;
    }

    /// <summary>Automatically finishes the auction, setting it as Success or Failed based on bids.</summary>
    /// <exception cref="InvalidAuctionException">Thrown if the auction is not in Active or Ending status.</exception>
    public void Finish(DateTime utcNow)
    {
        if (Status == AuctionStatus.Success || Status == AuctionStatus.Failed || Status == AuctionStatus.Cancelled) return;

        if (Status != AuctionStatus.Active && Status != AuctionStatus.Ending)
            throw new InvalidAuctionException("Only Active or Ending auctions can be finished.");

        var winningBid = _bids.FirstOrDefault(b => b.Status == BidStatus.Winning);

        EndedAt ??= utcNow;

        if (winningBid != null)
        {
            winningBid.SetAsWinner(utcNow);
            Status = AuctionStatus.Success;
        }
        else Status = AuctionStatus.Failed;

        Version++;
    }

    /// <summary> Cancels the auction if it's possible to or when the product is previously bought. </summary>
    /// <exception cref="InvalidAuctionException"> Thrown if the auction is not active or have bids placed already. </exception>
    public void Cancel(DateTime utcNow)
    {
        if (Status != AuctionStatus.Active && Status != AuctionStatus.Ending)
            throw new InvalidAuctionException("Its only possible to cancel an active/ending auction.");

        if (Bids.Count != 0) throw new InvalidAuctionException("It's not possible to cancel an auction that has bids");

        if (IsProcessingBid) throw new InvalidAuctionException("Cant cancel an auction when a bid attempts is being processed.");

        EndedAt ??= utcNow;

        Status = AuctionStatus.Cancelled;
        Version++;
    }

    /// <summary> Edits the auction settings if the auction is in the Awaiting status and has no bids. </summary>
    /// <exception cref="InvalidAuctionException"> Thrown if the auction is not in the Awaiting status or if the auction already has bids. </exception>
    /// <exception cref="InvalidAuctionSettingsException"> Thrown if the new settings are invalid according to AuctionSettings validation rules. </exception>
    public void EditSettings(DateTime utcNow, decimal? startBidValue = null, decimal? winBidValue = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        if (Status != AuctionStatus.Awaiting) throw new InvalidAuctionException("Only Awaiting auctions can have their settings edited.");

        if (Bids.Count > 0) throw new InvalidAuctionException("An auction with existing bids cannot have its settings edited.");

        if (IsProcessingBid) throw new InvalidAuctionException("Cant edit auction settings when a bid attempts is being processed.");

        Settings = AuctionSettings.Create(
            startBidValue ?? Settings.StartBidValue,
            winBidValue ?? Settings.WinBidValue,
            startDate ?? Settings.StartDate,
            endDate ?? Settings.EndDate,
            utcNow);

        EditedAt = utcNow;
        Version++;
    }

    /// <summary> Validates and prepares a new bid for the auction. </summary>
    /// <exception cref="InvalidAuctionException">Thrown when the auction is not active or the bid is invalid.</exception>
    /// <exception cref="InvalidBidException">Thrown when the bid value is invalid (e.g., exceeds allowed maximum).</exception>
    public void PrepareNewBid(decimal bidValue)
    {
        if (Status != AuctionStatus.Active && Status != AuctionStatus.Ending)
            throw new InvalidAuctionException("Bids can only be placed on an active or ending auction.");

        decimal minimumBid = GetMinimumNextBidValue();

        if (bidValue < minimumBid) throw new InvalidBidException($"Your bid must be at least {minimumBid:C}.");

        if (bidValue > Settings.WinBidValue) throw new InvalidBidException($"Bid cannot exceed the WinBidValue of {Settings.WinBidValue:C}.");
    }

    public void AbortNewBid()
    {
        if (IsProcessingBid is false)
            throw new Exception("Cant abort a new bid when there is no bid being processed.");

        IsProcessingBid = false;
    }

    public void CompleteNewBid(Guid bidderId, Guid paymentId, decimal bidValue, DateTime utcNow)
    {
        if (IsProcessingBid is false)
            throw new Exception("Cant complete a new bid when there is no bid being processed.");

        Bid bid = Bid.Create(
            auctionId: Id, 
            bidderId: bidderId, 
            paymentId: paymentId, 
            value: bidValue, 
            utcNow: utcNow);

        _bids.Add(bid);

        IsProcessingBid = false;
    }

    public void ExtendAuction()
    {
        Settings = Settings.ExtendEndDate();
        Version++;
    }

    public decimal GetMinimumNextBidValue()
    {
        var currentWinningBid = _bids.FirstOrDefault(b => b.Status == BidStatus.Winning);

        decimal nextBid = (currentWinningBid != null)
            ? Math.Ceiling(currentWinningBid.Value * 1.05m)
            : Settings.StartBidValue;

        if (nextBid > Settings.WinBidValue)
            nextBid = Settings.WinBidValue;

        return nextBid;
    }
}