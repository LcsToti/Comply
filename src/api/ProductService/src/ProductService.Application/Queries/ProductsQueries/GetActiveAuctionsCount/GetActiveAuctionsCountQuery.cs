using MediatR;

namespace ProductService.Application.Queries.ProductsQueries.GetActiveAuctionsCount;

public record GetActiveAuctionsCountQuery() : IRequest<long>;