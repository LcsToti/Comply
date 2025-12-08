using MediatR;

namespace ProductService.Application.Commands.ImagesCommands.RemoveImage;

public record RemoveImageCommand(
    Guid ProductId,
    Guid SellerId,
    List<string> ImageUrls) : IRequest;