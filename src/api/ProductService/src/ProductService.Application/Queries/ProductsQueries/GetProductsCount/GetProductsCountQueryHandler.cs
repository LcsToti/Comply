using MediatR;
using ProductService.Domain.Contracts;

namespace ProductService.Application.Queries.ProductsQueries.GetProductsCount;

public class GetProductsCountQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductsCountQuery, long>
{
    private readonly IProductRepository _productRepository = productRepository;
    public async Task<long> Handle(GetProductsCountQuery request, CancellationToken cancellationToken)
    {
        var count = await _productRepository.GetProductsCountAsync();
        return count;
    }
}