using MediatR;
using ProductService.Application.Responses;
using ProductService.Domain.Contracts;

namespace ProductService.Application.Queries.ProductsQueries.GetById;

public class GetProductByIdQueryHandler(
    IProductRepository productRepository) : IRequestHandler<GetProductByIdQuery, ProductResponse>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId) 
            ?? throw new KeyNotFoundException($"Product with ID {request.ProductId} was not found.");
        
        return product.ToProductResponse();
    }
}
