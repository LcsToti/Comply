using MediatR;
using ProductService.Application.Contracts;
using ProductService.Application.Contracts.Requests;
using ProductService.Domain.Contracts;
using System.Text.Json;

namespace ProductService.Application.Commands.ImagesCommands.AddImages;

public class AddImagesCommandHandler(
    IProductRepository productRepository,
    IFileUploaderService fileUploaderService,
    IAiService aiService) : IRequestHandler<AddImagesCommand>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IFileUploaderService _fileUploaderService = fileUploaderService;
    private readonly IAiService _aiService = aiService;

    public async Task Handle(AddImagesCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId)
            ?? throw new Exception("Product not found.");

        if (product.SellerId != request.SellerId)
            throw new UnauthorizedAccessException("Only the seller can add images.");

        var response = await _aiService.AnalyzeProductAsync(product, request.ImageUrls);

        var analysisResult = JsonSerializer.Deserialize<GeminiModerationResponseDTO>(response)
            ?? throw new Exception("Failed to deserialize AI moderation response.");

        if (!analysisResult.ModerationPassed)
        {
            throw new Exception($"Product moderation failed: " +
                $"- Image Reason: {analysisResult.ImageRejectionReason}");
        }

        var imageUrls = await _fileUploaderService.UploadImagesAsync(request.ImageUrls, "products");

        product.AddImages(request.SellerId, imageUrls);
        await _productRepository.UpdateAsync(product);
    }
}