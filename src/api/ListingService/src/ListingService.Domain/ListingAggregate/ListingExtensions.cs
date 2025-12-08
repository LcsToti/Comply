namespace ListingService.Domain.ListingAggregate;

public static class ListingExtensions
{
    public static ListingStatus ToggleVisibilityStatuses(this ListingStatus status) =>
        status == ListingStatus.Available ? ListingStatus.Paused : ListingStatus.Available;
}