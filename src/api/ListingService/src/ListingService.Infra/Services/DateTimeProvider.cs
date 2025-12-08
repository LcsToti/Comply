using ListingService.App.Common.Interfaces;

namespace ListingService.Infra.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
