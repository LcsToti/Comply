using MediatR;
using ProductService.Application.Responses;

namespace ProductService.Application.Queries.ProductsQueries.GetMyBiddedProducts;

public record GetMyBiddedProductsQuery(Guid UserId, int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedList<ProductResponse>>;