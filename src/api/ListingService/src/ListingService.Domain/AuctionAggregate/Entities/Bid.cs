using ListingService.Domain.AuctionAggregate.Enums;
using ListingService.Domain.Exceptions;
using ListingService.Domain.Common;

namespace ListingService.Domain.AuctionAggregate.Entities;

public class Bid
{
    public Guid Id { get; private set; }
    public Guid AuctionId { get; private set; }
    public Guid BidderId { get; private set; }
    public Guid PaymentId { get; private set; }
    public decimal Value { get; private set; }
    public BidStatus Status { get; private set; }
    public DateTime BiddedAt { get; private set; }
    public DateTime? OutbiddedAt { get; private set; }
    public DateTime? WonAt { get; private set; }

    private Bid(Guid auctionId, Guid bidderId, Guid paymentId, decimal value, DateTime biddedAt)
    {
        Id = Guid.NewGuid();
        AuctionId = auctionId;
        BidderId = bidderId;
        PaymentId = paymentId;
        Value = value;
        Status = BidStatus.Winning;
        BiddedAt = biddedAt;
    }

    #region For ORM
    private Bid() { }

    internal static Bid Restore(
        Guid id,
        Guid auctionId,
        Guid bidderId,
        Guid paymentId,
        decimal bidValue,
        BidStatus status,
        DateTime biddedAt,
        DateTime? outbiddedAt,
        DateTime? wonAt)
    {
        return new Bid
        {
            Id = id,
            AuctionId = auctionId,
            BidderId = bidderId,
            PaymentId = paymentId,
            Value = bidValue,
            Status = status,
            BiddedAt = biddedAt,
            OutbiddedAt = outbiddedAt,
            WonAt = wonAt
        };
    }
    #endregion

    /// <summary>Marks the current bid as outbid.</summary>
    /// <exception cref="InvalidBidException">Thrown if the bid is not in a valid state to be marked as outbid.</exception>
    internal static Bid Create(Guid auctionId, Guid bidderId, Guid paymentId, decimal value, DateTime utcNow)
    {
        if (value < DomainConstants.MinTransactionValue)
            throw new InvalidBidException($"Bid value must be at least {DomainConstants.MinTransactionValue:C}.");
        if (value > DomainConstants.MaxTransactionValue)
            throw new InvalidBidException($"Bid value should not exceed {DomainConstants.MaxTransactionValue:C}.");
        var biddedAt = utcNow;

        return new Bid(
            auctionId: auctionId,
            bidderId: bidderId,
            paymentId: paymentId,
            value: value,
            biddedAt: biddedAt);
    }

    #region Methods
    /// <summary>Marks the bid as outbid.</summary>
    /// <exception cref="InvalidBidException">Thrown if the bid is not in a valid state to be marked as outbid.</exception>
    public void MarkAsOutbid(DateTime utcNow)
    {
        if (Status != BidStatus.Winning)
            throw new InvalidBidException("Only the winning bid can be marked as outbid.");

        Status = BidStatus.Outbid;
        OutbiddedAt = utcNow;
    }

    /// <summary>Marks the bid as the winner.</summary>
    /// <exception cref="InvalidBidException">Thrown if the bid is not in a valid state to be marked as winner.</exception>
    internal void SetAsWinner(DateTime utcNow)
    {
        if (Status != BidStatus.Winning)
            throw new InvalidBidException("Only the winning bid can be marked as winner.");

        Status = BidStatus.Winner;
        WonAt = utcNow;
    }
    #endregion
}