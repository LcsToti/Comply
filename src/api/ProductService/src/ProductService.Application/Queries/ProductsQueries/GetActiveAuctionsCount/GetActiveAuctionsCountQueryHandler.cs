using MediatR;
using ProductService.Domain.Contracts;

namespace ProductService.Application.Queries.ProductsQueries.GetActiveAuctionsCount;

public class GetActiveAuctionsCountQueryHandler(IProductRepository productRepository) : IRequestHandler<GetActiveAuctionsCountQuery, long>
{
    private readonly IProductRepository _productRepository = productRepository;
    
    public async Task<long> Handle(GetActiveAuctionsCountQuery request, CancellationToken cancellationToken)
    {
        var count = await _productRepository.GetActiveAuctionsCountAsync();
        return count;
    }
}