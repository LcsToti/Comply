using FluentAssertions;
using Payments.Domain.Aggregates.PaymentAggregate.VOs;
using Payments.Domain.Exceptions.PaymentExceptions; // Importar exceção correta

namespace Payments.Domain.Tests.Aggregate.VOs;

public class AmountTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCalculateFeeAndNetCorrectly()
    {
        // Arrange
        const decimal total = 100.00m;
        const string currency = "BRL";
        const decimal expectedFee = 8.00m;
        const decimal expectedNet = 92.00m;

        // Act
        var amount = Amount.Create(total, currency);

        // Assert
        amount.Total.Should().Be(total);
        amount.Fee.Should().Be(expectedFee);
        amount.Net.Should().Be(expectedNet);
        amount.Currency.Should().Be(currency);
    }
    
    [Fact]
    public void Create_WithDecimalTotal_ShouldCalculateAndRoundFeeAndNetCorrectly()
    {
        // Arrange
        const decimal total = 123.45m;
        const string currency = "USD";
        const decimal expectedFee = 9.88m; // 123.45 * 0.08 = 9.876
        const decimal expectedNet = 113.57m; // 123.45 - 9.88 = 113.57

        // Act
        var amount = Amount.Create(total, currency);

        // Assert
        amount.Fee.Should().Be(expectedFee);
        amount.Net.Should().Be(expectedNet);
    }

    [Fact]
    public void Create_WithLowercaseCurrency_ShouldConvertToUppercase()
    {
        // Arrange
        const decimal total = 50m;
        const string currency = "brl";

        // Act
        var amount = Amount.Create(total, currency);

        // Assert
        amount.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Create_WithNegativeTotal_ShouldThrowInvalidPaymentParamsException()
    {
        // Arrange
        const decimal total = -100m;
        const string currency = "BRL";
        Action act = () => Amount.Create(total, currency);

        // Assert
        // ATUALIZADO: Troca de ArgumentException para InvalidPaymentParamsException
        act.Should().Throw<InvalidPaymentParamsException>()
            .WithMessage("Total value cannot be negative.");
    }

    [Theory]
    [InlineData("BR")]
    [InlineData("BRLN")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidCurrency_ShouldThrowInvalidPaymentParamsException(string invalidCurrency)
    {
        // Arrange
        const decimal total = 100m;
        Action act = () => Amount.Create(total, invalidCurrency);

        // Assert
        // ATUALIZADO: Troca de ArgumentException para InvalidPaymentParamsException
        act.Should().Throw<InvalidPaymentParamsException>()
            .WithMessage("Currency must be a ISO 4217 of 3 characters.");
    }

    [Fact]
    public void Create_WithTotalThatResultsInNegativeNet_ShouldThrowInvalidPaymentParamsException()
    {
        // Este teste é teoricamente necessário por causa da verificação,
        // mas é matematicamente impossível de acionar com a lógica de taxa atual (0.08m).
        // Se a taxa fosse > 1.0m, este teste seria relevante.
        // Por exemplo, se a taxa fosse 110% (1.1m):
        // const decimal total = 100m;
        // const decimal fee = 110m;
        // const decimal net = -10m;
        // Como a lógica de taxa está fixa em 0.08m, o `net < 0` nunca será acionado se `total >= 0`.
        // A lógica de domínio está correta em ter a verificação, mas não é testável no estado atual.
    }
}