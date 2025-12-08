using Microsoft.AspNetCore.Http;

namespace ProductService.Application.Contracts
{
    public interface IFileUploaderService
    {
        Task<string> UploadImageAsync(Stream fileStream, string fileName, string folder);

        Task<List<string>> UploadImagesAsync(List<IFormFile> fileStreams, string folder);

        Task DeleteImageAsync(List<string> fileUrls);
    }
}
