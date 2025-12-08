using MediatR;
using ProductService.Application.Responses;
using ProductService.Domain.Entities;

namespace ProductService.Application.Queries.ProductsQueries.GetById;

public record GetProductByIdQuery(Guid ProductId) : IRequest<ProductResponse>;
