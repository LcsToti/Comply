using MediatR;

namespace ProductService.Application.Commands.ImagesCommands.ReorderImages;

public record ReorderImagesCommand(
    Guid ProductId,
    Guid SellerId,
    List<string> OrderedImages) : IRequest;