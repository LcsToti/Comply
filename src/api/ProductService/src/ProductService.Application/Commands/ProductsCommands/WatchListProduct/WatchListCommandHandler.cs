using MediatR;
using ProductService.Domain.Contracts;

namespace ProductService.Application.Commands.ProductsCommands.WatchListProduct;

public class IncrementWatchListCommandHandler(IProductRepository productRepository) : IRequestHandler<IncrementWatchListCommand>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task Handle(IncrementWatchListCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId) 
            ?? throw new Exception("Product not found");

        product.IncrementWatchList();
        await _productRepository.UpdateWatchListCountAsync(product.ProductId, product.WatchListCount);
    }
}

public class DecrementWatchListCommandHandler(IProductRepository productRepository) : IRequestHandler<DecrementWatchListCommand>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task Handle(DecrementWatchListCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId) 
            ?? throw new Exception("Product not found");

        product.DecrementWatchList();
        await _productRepository.UpdateWatchListCountAsync(product.ProductId, product.WatchListCount);
    }
}