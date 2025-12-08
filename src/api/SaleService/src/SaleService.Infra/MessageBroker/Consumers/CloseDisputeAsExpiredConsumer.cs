using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SalesService.App.Events.SaleEvents.CloseDisputeAsExpired;

namespace SalesService.Infra.MessageBroker.Consumers;

public class CloseDisputeAsExpiredConsumer : IConsumer<CloseDisputeAsExpiredEvent>
{
    private readonly ILogger<CloseDisputeAsExpiredConsumer> _logger;
    private readonly IMediator _mediator;
    public CloseDisputeAsExpiredConsumer(ILogger<CloseDisputeAsExpiredConsumer> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<CloseDisputeAsExpiredEvent> context)
    {
        var msg = context.Message;

        _logger.LogInformation("Received CloseDisputeAsExpiredEvent");
        await _mediator.Send(new CloseDisputeAsExpiredEvent(msg.SaleId, msg.ExpiresAt));
    }
}