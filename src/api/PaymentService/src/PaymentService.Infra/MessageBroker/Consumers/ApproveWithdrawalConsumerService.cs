using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Payments.App.UseCases.PayoutCases.ApprovePaymentWithdrawal;
using Shared.Contracts.Messages.PaymentsService;

namespace Payments.Infra.MessageBroker.Consumers;
public class ApproveWithdrawalConsumerService : IConsumer<ApproveWithdrawalMessage>
{
    private readonly ILogger<ApproveWithdrawalConsumerService> _logger;
    private readonly IMediator _mediator;

    public ApproveWithdrawalConsumerService(ILogger<ApproveWithdrawalConsumerService> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    public async Task Consume(ConsumeContext<ApproveWithdrawalMessage> context)
    {
        var msg = context.Message;
        
        _logger.LogInformation("Retirada aprovada para o pagamento: {PaymentId} ", msg.PaymentId);
        await _mediator.Send(new ApprovePaymentWithdrawalCommand(msg.PaymentId));
    }
}