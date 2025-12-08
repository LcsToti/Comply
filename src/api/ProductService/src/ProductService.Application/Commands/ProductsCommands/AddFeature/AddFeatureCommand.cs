using MediatR;

namespace ProductService.Application.Commands.ProductsCommands.AddFeature
{
    public record AddFeatureCommand(
        Guid ProductId,
        Guid SellerId,
        int DurationInDays) : IRequest;
}