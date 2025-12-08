using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SalesService.App.Events.SaleEvents.CreateSale;
using Shared.Contracts.Messages.ListingService.Sales;

namespace SalesService.Infra.MessageBroker.Consumers;

public class CreateSaleConsumer : IConsumer<CreateSaleMessage>
{
    private readonly ILogger<CreateSaleConsumer> _logger;
    private readonly IMediator _mediator;
    public CreateSaleConsumer(ILogger<CreateSaleConsumer> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<CreateSaleMessage> context)
    {
        var msg = context.Message;
        
        _logger.LogInformation("Received CreateSaleMessage");
        await _mediator.Send(new CreateSaleEvent(
            msg.ProductId,
            msg.BuyerId,
            msg.SellerId,
            msg.ListingId,
            msg.PaymentId,
            msg.ProductValue
        ));
    }
}