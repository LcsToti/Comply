namespace ProductService.API.Requests.Images;

public record ImagesRequest(
    List<IFormFile> ImageUrls);

public record ImagesUrlsRequest(
    List<string> ImageUrls);