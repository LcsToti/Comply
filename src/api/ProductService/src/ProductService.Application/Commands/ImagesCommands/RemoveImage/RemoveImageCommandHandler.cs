using MediatR;
using ProductService.Application.Contracts;
using ProductService.Domain.Contracts;

namespace ProductService.Application.Commands.ImagesCommands.RemoveImage;

public class RemoveImageCommandHandler(IProductRepository productRepository, IFileUploaderService fileUploaderService) : IRequestHandler<RemoveImageCommand>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IFileUploaderService _fileUploaderService = fileUploaderService;

    public async Task Handle(RemoveImageCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId)
            ?? throw new Exception("Product not found.");

        if (product.SellerId != request.SellerId)
            throw new UnauthorizedAccessException("Only the seller can remove images.");
        
        await _fileUploaderService.DeleteImageAsync(request.ImageUrls);

        product.RemoveImage(request.SellerId, request.ImageUrls);

        await _productRepository.UpdateAsync(product);
    }
}