using MediatR;

namespace ProductService.Application.Queries.ProductsQueries.GetProductsCount;

public record GetProductsCountQuery() : IRequest<long>;