namespace ListingService.App.Common.Errors;

public record InvalidListingStatus(string CurrentStatus) : IError
{
    public string Message => $"It's not possible to create an auction because the current status is '{CurrentStatus}', but the required status is 'Available'.";
}