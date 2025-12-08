namespace ListingService.App.Common.Errors;

public record InvalidBidOperation(string Message) : IError { }