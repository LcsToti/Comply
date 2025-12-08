
using Payments.Domain.Exceptions.PaymentExceptions;

namespace Payments.Domain.Aggregate.VOs
{
    public record Timestamps
    {
        public DateTime CreatedAt { get; }
        public DateTime? ProcessedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public DateTime? WithdrawnAt { get; init; }

        private Timestamps(DateTime createdAt, DateTime? processedAt, DateTime updatedAt, DateTime? withdrawnAt)
        {
            CreatedAt = createdAt;
            ProcessedAt = processedAt;
            UpdatedAt = updatedAt;
            WithdrawnAt = withdrawnAt;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Timestamps"/> record with the specified timestamps.
        /// </summary>
        /// <param name="createdAt">The creation timestamp. Must not be in the future.</param>
        /// <param name="processedAt">The processing timestamp. Must not be earlier than <paramref name="createdAt"/> and, if provided, must be earlier than <paramref name="updatedAt"/>.</param>
        /// <param name="updatedAt">The updated timestamp. Must not be earlier than <paramref name="createdAt"/>, and must not be earlier than <paramref name="processedAt"/> if it is provided.</param>
        /// <param name="withdrawnAt">The withdrawal timestamp. If provided, it must not be earlier than <paramref name="createdAt"/> or <paramref name="processedAt"/>.</param>
        /// <returns>A new instance of the <see cref="Timestamps"/> record with the specified timestamps.</returns>
        /// <exception cref="InvalidPaymentParamsException">
        /// Thrown when any of the following conditions are met:
        /// - <paramref name="createdAt"/> is in the future.
        /// - <paramref name="updatedAt"/> is earlier than <paramref name="createdAt"/>.
        /// - <paramref name="processedAt"/> is earlier than <paramref name="createdAt"/>.
        /// - <paramref name="updatedAt"/> is earlier than <paramref name="processedAt"/>.
        /// - <paramref name="withdrawnAt"/> is earlier than <paramref name="createdAt"/> or <paramref name="processedAt"/>.
        /// </exception>
        public static Timestamps Create(DateTime createdAt, DateTime? processedAt, DateTime updatedAt,
            DateTime? withdrawnAt)
        {
            if (createdAt > DateTime.UtcNow)
                throw new InvalidPaymentParamsException("CreatedAt cannot be in the future.");

            if (updatedAt < createdAt)
                throw new InvalidPaymentParamsException("UpdatedAt cannot be earlier than CreatedAt.");

            if (processedAt.HasValue && processedAt.Value < createdAt)
                throw new InvalidPaymentParamsException("ProcessedAt cannot be earlier CreatedAt.");

            if (processedAt.HasValue && updatedAt < processedAt.Value)
                throw new InvalidPaymentParamsException("UpdatedAt cannot be earlier than ProcessedAt.");
            
            if (withdrawnAt.HasValue && withdrawnAt.Value < createdAt)
                throw new InvalidPaymentParamsException("WithdrawnAt cannot be earlier than CreatedAt.");
            
            if (withdrawnAt.HasValue && processedAt.HasValue && withdrawnAt.Value < processedAt.Value)
                throw new InvalidPaymentParamsException("WithdrawnAt cannot be earlier than ProcessedAt.");
            
            return new Timestamps(createdAt, processedAt, updatedAt, withdrawnAt);
        }
    }
}
