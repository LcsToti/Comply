using Payments.Domain.Aggregate;
using Payments.Domain.Aggregate.VOs;
using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.Aggregates.PaymentAggregate.Factories;
using Payments.Domain.Aggregates.PaymentAggregate.VOs;
using Payments.Infra.Persistence.DataModel;

namespace Payments.Infra.Persistence.Mappers
{
    public static class PaymentMapper
    {
        #region Domain to DataModel
        
        public static RefundDataModel ToDataModel(Refund refund)
        {
            return new RefundDataModel
            {
                Id = refund.Id,
                ApiRefundId = refund.ApiRefundId,
                RefundAmount = refund.RefundAmount,
                Reason = refund.Reason,
                RefundStatus = refund.RefundStatus,
                CreatedAt = refund.CreatedAt
            };
        }
        private static AmountDataModel ToDataModel(Amount amount)
        {
            return new AmountDataModel
            {
                Total = amount.Total,
                Fee = amount.Fee,
                Net = amount.Net,
                Currency = amount.Currency
            };
        }
        private static GatewayDataModel ToDataModel(Gateway gateway)
        {
            return new GatewayDataModel
            {
                Name = gateway.Name,
                ApiPaymentId = gateway.ApiPaymentId,
                ApiChargeId = gateway.ApiChargeId
            };
        }
        private static TimestampsDataModel ToDataModel(Timestamps timestamps)
        {
            return new TimestampsDataModel
            {
                CreatedAt = timestamps.CreatedAt,
                ProcessedAt = timestamps.ProcessedAt,
                UpdatedAt = timestamps.UpdatedAt
            };
        }
        public static PaymentDataModel ToDataModel(Payment payment)
        {
            if (payment == null) return null;

            return new PaymentDataModel
            {
                Id = payment.Id,
                WithdrawalStatus = payment.WithdrawalStatus,
                Status = payment.Status,
                PaymentMethod = payment.PaymentMethod,
                Amount = ToDataModel(payment.Amount),
                Gateway = ToDataModel(payment.Gateway),
                PayerId = payment.PayerId,
                SellerId = payment.SellerId,
                SourceId = payment.SourceId,
                Timestamps = ToDataModel(payment.Timestamps),
                Refunds = payment.Refunds?.Select(ToDataModel).ToList() ?? []
            };
        }
        #endregion

        #region DataModel to Domain
        public static Refund ToDomain(RefundDataModel model)
        {
            return RefundFactory.Create(
                model.ApiRefundId,
                model.RefundAmount,
                model.Reason,
                model.RefundStatus,
                model.CreatedAt
            );
        }
        public static Payment ToDomain(PaymentDataModel model)
        {
            if (model == null) return null;

            var amount = Amount.Create(model.Amount.Total, model.Amount.Currency);
            var gateway = Gateway.Create(model.Gateway.Name, model.Gateway.ApiPaymentId, model.Gateway.ApiChargeId);
            var timestamps = Timestamps.Create(model.Timestamps.CreatedAt, model.Timestamps.ProcessedAt, model.Timestamps.UpdatedAt, model.Timestamps.WithdrawnAt);
            var refunds = model.Refunds.Select(ToDomain);
            
            return PaymentFactory.LoadFromState(
                model.Id,
                model.Status,
                model.WithdrawalStatus,
                model.PaymentMethod,
                amount,
                gateway,
                model.PayerId,
                model.SellerId,
                model.SourceId,
                timestamps,
                refunds
            );
        }
        
        #endregion
    }
}