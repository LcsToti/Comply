using MediatR;
using ProductService.Domain.Contracts;

namespace ProductService.Application.Commands.ProductsCommands.AddFeature;

public class AddFeatureCommandHandler(IProductRepository productRepository) : IRequestHandler<AddFeatureCommand>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task Handle(AddFeatureCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId) 
            ?? throw new Exception("Product not found.");

        if (product.SellerId != request.SellerId)
            throw new UnauthorizedAccessException("Only the seller can add a featured product.");
        
        product.AddFeatured(request.SellerId, request.DurationInDays);

        await _productRepository.UpdateAsync(product);
    }
}