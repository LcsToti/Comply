using Payments.Domain.Aggregates.PaymentAccountAggregate.Entity;
using Payments.Domain.Aggregates.PaymentAccountAggregate.Factories;
using Payments.Infra.Persistence.DataModel;

namespace Payments.Infra.Persistence.Mappers;

public static class PaymentAccountMapper
{
    public static PaymentAccountDataModel ToDataModel(PaymentAccount paymentAccount)
    {
        return new PaymentAccountDataModel
        {
            UserId = paymentAccount.UserId,
            CustomerId = paymentAccount.CustomerId,
            ConnectedAccountId = paymentAccount.ConnectedAccountId,
            AccountStatus = paymentAccount.AccountStatus       
        };
    }
    public static PaymentAccount ToDomain(PaymentAccountDataModel paymentAccountDataModel)
    {
        return PaymentAccountFactory.Create(
            paymentAccountDataModel.UserId,
            paymentAccountDataModel.CustomerId,
            paymentAccountDataModel.ConnectedAccountId,
            paymentAccountDataModel.AccountStatus);
    }   
}