using MediatR;
using ProductService.Application.Responses;

namespace ProductService.Application.Queries.ProductsQueries.GetMyOutbidProducts;

public record GetMyOutbidProductsQuery(Guid UserId, int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedList<ProductResponse>>;