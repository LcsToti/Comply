namespace ListingService.App.Common.Errors;

public record InvalidPurchaseOperation(string Message) : IError { }