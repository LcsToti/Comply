namespace ListingService.App.Common.Errors;

public record NotFound(string ResourceName, object Id) : IError
{
    public string Message => $"The resource '{ResourceName}' with ID '{Id}' wasn't found.";
}