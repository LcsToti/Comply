using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Payments.App.UseCases.UserCases.CreatePaymentAccount;
using Shared.Contracts.Messages.UserService;

namespace Payments.Infra.MessageBroker.Consumers;
public class CreatePaymentAccountConsumerService : IConsumer<CreatedUserMessage>
{
    private readonly ILogger<CreatePaymentAccountConsumerService> _logger;
    private readonly IMediator _mediator;

    public CreatePaymentAccountConsumerService(ILogger<CreatePaymentAccountConsumerService> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<CreatedUserMessage> context)
    {
        var msg = context.Message;
        
        _logger.LogInformation("Criando uma conta de pagamento para o cliente {Name} com o email: {Email}", msg.Name, msg.Email);
        await _mediator.Send(new CreatePaymentAccountEvent(msg.Email, msg.Name, msg.UserId));
    }
}