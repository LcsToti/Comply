using MediatR;
using Microsoft.AspNetCore.Http;

namespace ProductService.Application.Commands.ImagesCommands.AddImages;

public record AddImagesCommand(
    Guid ProductId,
    Guid SellerId,
    List<IFormFile> ImageUrls) : IRequest; 