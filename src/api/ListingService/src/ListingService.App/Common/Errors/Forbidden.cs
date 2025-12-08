namespace ListingService.App.Common.Errors;

public record Forbidden(string Message) : IError { }