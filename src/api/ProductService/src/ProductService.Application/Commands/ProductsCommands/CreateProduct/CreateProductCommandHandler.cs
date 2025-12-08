using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Contracts;
using ProductService.Application.Contracts.Requests;
using ProductService.Application.Responses;
using ProductService.Domain.Contracts;
using ProductService.Domain.Factories;
using System.Text.Json;

namespace ProductService.Application.Commands.ProductsCommands.CreateProduct;

public class CreateProductCommandHandler(
    IProductRepository productRepository,
    IFileUploaderService fileUploaderService,
    IAiService aiService,
    ILogger<CreateProductCommandHandler> logger) : IRequestHandler<CreateProductCommand, ProductResponse>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IFileUploaderService _fileUploaderService = fileUploaderService;
    private readonly IAiService _aiService = aiService;
    private readonly ILogger<CreateProductCommandHandler> _logger = logger;

    public async Task<ProductResponse> Handle(CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = ProductFactory.Create(
            request.SellerId,
            request.Title,
            request.Description,
            request.Locale,
            request.Characteristics,
            request.Condition,
            request.Category,
            request.DeliveryPreference);

        _logger.LogInformation("Created product entity: {Product}", product);

        GeminiModerationResponseDTO? analysisResult;

        if (request is { IsTest: true, UserRole: "Admin" or "Moderator" })
        {
            _logger.LogInformation("Skipping AI moderation for test product by Admin/Moderator.");
            analysisResult = new GeminiModerationResponseDTO
            {
                ModerationPassed = true,
                TextRejectionReason = "NONE",
                ImageRejectionReason = "NONE"
            };
        }
        else
        {
            _logger.LogInformation("Sending product data to AI service for analysis.");
            var response = await _aiService.AnalyzeProductAsync(product, request.ImageUrls)
                           ?? throw new Exception("AI service did not return a response.");

            _logger.LogInformation("Received AI analysis response: {Response}", response);

            analysisResult = JsonSerializer.Deserialize<GeminiModerationResponseDTO>(response)
                 ?? throw new Exception("Failed to deserialize AI moderation response.");

            _logger.LogInformation("Deserialized AI analysis result: {AnalysisResult}", analysisResult);
        }

        if (!analysisResult.ModerationPassed)
        {
            throw new Exception($"Product moderation failed: " +
                $"- Text Reason: {analysisResult.TextRejectionReason};" +
                $"- Image Reason: {analysisResult.ImageRejectionReason}");
        }

        var imageUrls = await _fileUploaderService.UploadImagesAsync(request.ImageUrls, $"products/{product.ProductId}/images");
        product.AddImages(request.SellerId, imageUrls);
        await _productRepository.AddAsync(product);


        return product.ToProductResponse();
    }
}