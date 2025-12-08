using MediatR;

namespace Payments.App.UseCases.UserCases.CreatePaymentAccount;

public record CreatePaymentAccountEvent(string Email, string Name, Guid UserId) : IRequest;