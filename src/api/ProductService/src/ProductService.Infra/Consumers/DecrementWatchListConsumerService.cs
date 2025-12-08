using MassTransit;
using MediatR;
using ProductService.Application.Commands.ProductsCommands.WatchListProduct;
using ProductService.Application.Enums;
using ProductService.Application.Events;
using Shared.Contracts.Messages.NotificationService;

namespace ProductService.Infrastructure.Consumers;

public class DecrementWatchListConsumerService(ISender sender) : IConsumer<DecrementWatchListMessage>
{
    private readonly ISender _sender = sender;
    public async Task Consume(ConsumeContext<DecrementWatchListMessage> context)
    {
        var message = context.Message;
        IRequest command = new DecrementWatchListCommand(message.ProductId);
        await _sender.Send(command);
    }
}