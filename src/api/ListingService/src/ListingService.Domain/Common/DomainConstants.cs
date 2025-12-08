namespace ListingService.Domain.Common;

public static class DomainConstants
{
    public const decimal MaxTransactionValue = 50000;
    public const decimal MinTransactionValue = 5;

    public const int MinMinutesBeforeAuctionStarts = 0;
    public const int MinAuctionDurationMinutes = 30;
    public const int ExtendAuctionDurationMinutes = 10;

    public readonly static TimeSpan RemainingTimeToSetEnding = TimeSpan.FromMinutes(ExtendAuctionDurationMinutes);
}
