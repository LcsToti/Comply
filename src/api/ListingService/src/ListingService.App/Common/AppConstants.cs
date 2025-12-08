namespace ListingService.App.Common;

public static class AppConstants
{
    public static readonly TimeSpan TimeToLive = TimeSpan.FromSeconds(7);
    public static readonly TimeSpan TimeToProcess = TimeSpan.FromSeconds(5);
    public static readonly TimeSpan TotalMessageTimeWindow = TimeToProcess + TimeToLive;
    public const int MaxPageSize = 30;
}

