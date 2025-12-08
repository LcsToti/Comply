using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.App.Events.ProcessNotificationEvent;
using NotificationService.Domain.Enums;
using Shared.Contracts.Messages.PaymentsService;

namespace NotificationService.Infra.MessageBroker.Consumers.Payments.Payouts;

public class ApprovedToWithdrawalConsumer : IConsumer<ApproveWithdrawalMessage>{
    private readonly ILogger<ApprovedToWithdrawalConsumer> _logger;
    private readonly IMediator _mediator;
    
    public ApprovedToWithdrawalConsumer(ILogger<ApprovedToWithdrawalConsumer> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<ApproveWithdrawalMessage> context)
    {
        var msg = context.Message;

        const string message = "Um de seus pagamentos foi aprovado para saque.. Venha dar uma olhada";

        await _mediator.Send(new ProcessNotificationEvent(
            NotificationType.Payment,
            msg.UserId,
            message,
            msg.PaymentId
            ));
        _logger.LogInformation("Notification Sent -- Approved To Withdrawal");
    }
}