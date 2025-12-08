using MediatR;
using Shared.Contracts.Messages.ListingService.ProductReadModel;

namespace ProductService.Application.Commands.ProductsCommands.SetListingReadModel;

public record SetListingReadModelCommand(SetListingReadModelMessage Message) : IRequest;
