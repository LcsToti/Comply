using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Application.Commands.ProductsCommands.SetListingReadModel;
using Shared.Contracts.Messages.ListingService.ProductReadModel;

namespace ProductService.Infrastructure.Consumers;

public class ListingReadModelConsumer(
    IMediator mediator,
    ILogger<ListingReadModelConsumer> logger) : IConsumer<SetListingReadModelMessage>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<ListingReadModelConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<SetListingReadModelMessage> context)
    {
        await _mediator.Send(new SetListingReadModelCommand(context.Message));
        _logger.LogInformation("Processed SetListingReadModelMessage for ListingId: {ListingId}", context.Message.ListingId);
    }
}