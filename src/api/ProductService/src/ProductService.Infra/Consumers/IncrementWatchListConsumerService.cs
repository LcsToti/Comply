using MassTransit;
using MediatR;
using ProductService.Application.Commands.ProductsCommands.WatchListProduct;
using Shared.Contracts.Messages.NotificationService;

namespace ProductService.Infrastructure.Consumers;

public class IncrementWatchListConsumerService(ISender sender) : IConsumer<IncrementWatchListMessage>
{
    private readonly ISender _sender = sender;
    public async Task Consume(ConsumeContext<IncrementWatchListMessage> context)
    {
        var message = context.Message;
        IRequest command = new IncrementWatchListCommand(message.ProductId);
        await _sender.Send(command);
    }
}