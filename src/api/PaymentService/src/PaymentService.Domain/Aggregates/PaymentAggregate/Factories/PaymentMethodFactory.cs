using Payments.Domain.Aggregates.PaymentAggregate.Entities;
using Payments.Domain.Exceptions.PaymentMethodExceptions;

namespace Payments.Domain.Aggregates.PaymentAggregate.Factories;

public static class PaymentMethodFactory
{
    /// <summary>
    /// Creates a new instance of the <see cref="PaymentMethod"/> class with the specified parameters.
    /// </summary>
    /// <param name="id">The unique identifier for the payment method. Cannot be null or empty.</param>
    /// <param name="type">The type or category of the payment method (e.g., credit card, PayPal). Cannot be null or empty.</param>
    /// <param name="last4">The last four digits of the payment method, if applicable. Can be null.</param>
    /// <param name="brand">The brand of the payment method (e.g., Visa, Mastercard). Can be null.</param>
    /// <returns>A new instance of <see cref="PaymentMethod"/> initialized with the provided data.</returns>
    /// <exception cref="InvalidPaymentMethodParamsException">Thrown when <paramref name="id"/> or <paramref name="type"/> is null or empty.</exception>
    public static PaymentMethod Create(string id, string type, string? last4, string? brand)
    {
        if (string.IsNullOrEmpty(id))
            throw new InvalidPaymentMethodParamsException(nameof(id));

        if (string.IsNullOrEmpty(type))
            throw new InvalidPaymentMethodParamsException(nameof(type));
        
        return new PaymentMethod(id, type, last4, brand);
    }
}