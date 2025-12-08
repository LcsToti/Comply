using MediatR;

namespace ProductService.Application.Commands.ProductsCommands.WatchListProduct;

public record IncrementWatchListCommand(Guid ProductId) : IRequest;
public record DecrementWatchListCommand(Guid ProductId) : IRequest;