using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Domain.Contracts;

namespace ProductService.Application.Commands.ProductsCommands.SetListingReadModel;

public class SetListingReadModelCommandHandler(
    IProductRepository productRepository,
    ILogger<SetListingReadModelCommandHandler> logger) : IRequestHandler<SetListingReadModelCommand>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ILogger<SetListingReadModelCommandHandler> _logger = logger;

    public async Task Handle(SetListingReadModelCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling SetListingReadModelCommand for Product with ID {ProductId}", request.Message.ProductId);
        var msg = request.Message;
        
        var productId = msg.ProductId;

        var product = await _productRepository.GetByIdAsync(productId)
            ?? throw new Exception($"Product with ID {productId} not found.");

        product.SetListingReadModel(msg.ToListingReadModel());
        _logger.LogInformation("ListingReadModel object: {@ListingReadModel}", product.Listing);

        await _productRepository.UpdateAsync(product);
        _logger.LogInformation("ListingReadModel set for Product with ID {ProductId}", productId);
    }
}