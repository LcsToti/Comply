
using Payments.Domain.Exceptions.PaymentExceptions;

namespace Payments.Domain.Aggregates.PaymentAggregate.VOs
{
    public record Amount
    {
        public decimal Total { get; }
        public decimal Fee { get; }
        public decimal Net { get; }
        public string Currency { get; }

        private Amount(decimal total, decimal fee, decimal net, string currency)
        {
            Total = total;
            Fee = fee;
            Net = net;
            Currency = currency;
        }

        /// <summary>
        /// Creates an instance of the <see cref="Amount"/> record with the specified total and currency.
        /// </summary>
        /// <param name="total">The total amount to be processed. Must be non-negative.</param>
        /// <param name="currency">The 3-character ISO 4217 currency code. Must be valid and non-empty.</param>
        /// <returns>An instance of the <see cref="Amount"/> record representing the total, fee, net, and currency values.</returns>
        /// <exception cref="InvalidPaymentParamsException">
        /// Thrown when the total is negative, the currency is invalid or not a 3-character ISO 4217 string,
        /// or when the calculated net amount is negative.
        /// </exception>
        public static Amount Create(decimal total, string currency)
        {
            if (total < 0)
                throw new InvalidPaymentParamsException("Total value cannot be negative.");

            if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
                throw new InvalidPaymentParamsException("Currency must be a ISO 4217 of 3 characters.");

            const decimal defaultFeePercentage = 0.08m;
            var fee = Math.Round(total * defaultFeePercentage, 2);
            var net = total - fee;

            return net < 0
                ? throw new InvalidPaymentParamsException("Net cannot be negative. Verify total and fee values")
                : new Amount(total, fee, net, currency.ToUpper());
        }
    };
}
