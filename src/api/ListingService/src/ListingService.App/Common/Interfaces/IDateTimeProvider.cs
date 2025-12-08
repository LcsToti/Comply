namespace ListingService.App.Common.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
