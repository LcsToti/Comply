using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ProductService.Application.Contracts;
using ProductService.Domain.Contracts;

namespace ProductService.Infrastructure.Services
{
    public class S3FileUploader : IFileUploaderService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3FileUploader(IAmazonS3 s3Client, IConfiguration config)
        {
            _s3Client = s3Client;
            _bucketName = config["AWS:BucketName"]!;
        }

        public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string folder)
        {
            var transferUtility = new TransferUtility(_s3Client);
            var fileKey = $"{folder}/IMAGE-{Guid.NewGuid()}";

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = fileStream,
                Key = fileKey,
                BucketName = _bucketName,
            };

            uploadRequest.Headers.CacheControl = "max-age=31536000";

            await transferUtility.UploadAsync(uploadRequest);

            return $"https://{_bucketName}.s3.amazonaws.com/{fileKey}";
        }

        public async Task<List<string>> UploadImagesAsync(List<IFormFile> fileStreams, string folder)
        {
            var uploadTasks = fileStreams.Where(f => f.Length > 0)
                .Select(file => UploadImageAsync(file.OpenReadStream(), file.FileName, folder));

            var urls = await Task.WhenAll(uploadTasks);
            return [..urls];
        }

        public async Task DeleteImageAsync(List<string> fileUrls)
        {
            foreach (var key in fileUrls.Select(url => new Uri(url)).Select(uri => uri.AbsolutePath.TrimStart('/')))
            {
                var deleteRequest = new Amazon.S3.Model.DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };

                await _s3Client.DeleteObjectAsync(deleteRequest);
            }
        }
    }
}