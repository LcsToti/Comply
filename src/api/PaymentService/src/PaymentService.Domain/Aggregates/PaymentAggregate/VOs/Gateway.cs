
using Payments.Domain.Exceptions.PaymentExceptions;

namespace Payments.Domain.Aggregate.VOs
{
    public record Gateway
    {
        public string Name { get; }
        public string ApiPaymentId { get; }
        public string ApiChargeId { get; }

        private Gateway(string name, string apiPaymentId, string apiChargeId)
        {
            Name = name;
            ApiPaymentId = apiPaymentId;
            ApiChargeId = apiChargeId;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Gateway"/> record.
        /// </summary>
        /// <param name="name">The name of the gateway. Cannot be null or empty.</param>
        /// <param name="apiPaymentId">The API payment identifier. Cannot be null or empty.</param>
        /// <param name="apiChargeId">The API charge identifier. Cannot be null or empty.</param>
        /// <returns>A new instance of the <see cref="Gateway"/> record initialized with the specified parameters.</returns>
        /// <exception cref="InvalidPaymentParamsException">
        /// Thrown when <paramref name="name"/>, <paramref name="apiPaymentId"/>, or <paramref name="apiChargeId"/> is null, empty, or consists only of white-space characters.
        /// </exception>
        public static Gateway Create(string name, string apiPaymentId, string apiChargeId)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidPaymentParamsException("Gateway name cannot be null or empty.");
            }
            if (string.IsNullOrWhiteSpace(apiPaymentId))
            {
                throw new InvalidPaymentParamsException("API PaymentIntent ID cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(apiChargeId))
            {
                throw new InvalidPaymentParamsException("API Charge ID cannot be null or empty.");
            }
            
            return new Gateway(name, apiPaymentId, apiChargeId);
        }
    }
}
