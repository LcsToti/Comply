using MediatR;
using ProductService.Domain.Contracts;

namespace ProductService.Application.Commands.ImagesCommands.ReorderImages;

public class ReorderImagesCommandHandler(IProductRepository productRepository) : IRequestHandler<ReorderImagesCommand>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task Handle(ReorderImagesCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId) 
            ?? throw new Exception("Product not found.");

        if (product.SellerId != request.SellerId)
            throw new UnauthorizedAccessException("Only the seller can reorder images.");     

        product.ReorderImages(request.SellerId, request.OrderedImages);

       await _productRepository.UpdateAsync(product);
    }
}