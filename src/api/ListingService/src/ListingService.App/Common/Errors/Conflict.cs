namespace ListingService.App.Common.Errors;

public record Conflict(string Message) : IError { }