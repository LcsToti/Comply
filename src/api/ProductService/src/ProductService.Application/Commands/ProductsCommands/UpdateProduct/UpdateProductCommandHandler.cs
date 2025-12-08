using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Contracts;
using ProductService.Application.Contracts.Requests;
using ProductService.Application.Responses;
using ProductService.Domain.Contracts;
using System.Text.Json;

namespace ProductService.Application.Commands.ProductsCommands.UpdateProduct;

public class UpdateProductCommandHandler(
    IProductRepository productRepository,
    IAiService aiService,
    ILogger<UpdateProductCommandHandler> logger) : IRequestHandler<UpdateProductCommand, ProductResponse>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IAiService _aiService = aiService;
    private readonly ILogger<UpdateProductCommandHandler> _logger = logger;

    public async Task<ProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId)
            ?? throw new Exception("Product not found.");

        if (product.SellerId != request.SellerId)
            throw new Exception("SellerId does not match the product's SellerId.");

        if (request.Title != null) product.UpdateTitle(request.SellerId, request.Title);

        if (request.Description != null) product.UpdateDescription(request.SellerId, request.Description);

        if (request.Locale != null) product.UpdateLocale(request.SellerId, request.Locale);

        if (request.Characteristics != null) product.UpdateCharacteristics(request.SellerId, request.Characteristics);

        if (request.Condition != null) product.UpdateCondition(request.SellerId, request.Condition);

        if (request.Category != null) product.UpdateCategory(request.SellerId, request.Category);

        if (request.DeliveryPreference != null) product.UpdateDeliveryPreference(request.SellerId, request.DeliveryPreference);

        _logger.LogInformation("Updated product entity: {Product}", product);

        // Gemini
        _logger.LogInformation("Sending product data to AI service for analysis.");
        var response = await _aiService.AnalyzeProductAsync(product)
            ?? throw new Exception("AI service did not return a response.");

        _logger.LogInformation("Received AI analysis response: {Response}", response);
        var analysisResult = JsonSerializer.Deserialize<GeminiModerationResponseDTO>(response)
             ?? throw new Exception("Failed to deserialize AI moderation response.");

        _logger.LogInformation("Deserialized AI analysis result: {AnalysisResult}", analysisResult);

        if (!analysisResult.ModerationPassed)
        {
            throw new Exception($"Product moderation failed: " +
                $"- Text Reason: {analysisResult.TextRejectionReason};" +
                $"- Image Reason: {analysisResult.ImageRejectionReason}");
        }

        await _productRepository.UpdateAsync(product);

        return product.ToProductResponse();
    }
}
