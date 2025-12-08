using ListingService.Domain.Exceptions;

namespace ListingService.Api.Extensions;

public static class ExceptionExtensions
{
    public static (string Title, int StatusCode, string Message) ToProblemDetails(this Exception ex)
    {
        return ex switch
        {
            InvalidAuctionException => ("Invalid auction operation", 400, ex.Message),
            InvalidAuctionSettingsException => ("Invalid auction settings operation", 400, ex.Message),
            InvalidBidException => ("Invalid bid operation", 400, ex.Message),
            InvalidListingException => ("Invalid listing operation", 400, ex.Message),
            _ => ("Internal Server Error", 500, ex.Message)
        };
    }
}
