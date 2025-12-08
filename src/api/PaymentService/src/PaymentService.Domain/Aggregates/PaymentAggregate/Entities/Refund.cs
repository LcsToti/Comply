
namespace Payments.Domain.Aggregates.PaymentAggregate.Entities
{
    public class Refund 
    {
        public Guid Id { get; private set; }
        public string ApiRefundId { get; private set; }
        public decimal RefundAmount { get; private set; }
        public string Reason { get; private set; }
        public string RefundStatus { get; private set; }
        public DateTime CreatedAt { get; private set; }

        internal Refund(string apiRefundId, decimal amount, string reason, string status, DateTime createdAt)
        {
            Id = Guid.NewGuid();
            ApiRefundId = apiRefundId;
            RefundAmount = amount;
            Reason = reason;
            RefundStatus = status;
            CreatedAt = createdAt;
        }
    }
}
