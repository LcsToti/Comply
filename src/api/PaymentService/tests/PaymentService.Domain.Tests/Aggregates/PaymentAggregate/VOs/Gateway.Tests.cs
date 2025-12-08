using FluentAssertions;
using Payments.Domain.Aggregate.VOs;
using Payments.Domain.Exceptions.PaymentExceptions; // Importar exceção correta

namespace Payments.Domain.Tests.Aggregate.VOs;

public class GatewayTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateAndReturnGateway()
    {
        // Arrange
        const string name = "TestGateway";
        const string apiPaymentId = "pi-1231";
        const string apiChargeId = "ch-1231";

        // Act
        var gateway = Gateway.Create(name, apiPaymentId, apiChargeId);

        // Assert
        gateway.Name.Should().Be(name);
        gateway.ApiPaymentId.Should().Be(apiPaymentId);
        gateway.ApiChargeId.Should().Be(apiChargeId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldThrowInvalidPaymentParamsException(string invalidName)
    {
        // Arrange
        const string apiPaymentId = "pi-12345";
        const string apiChargeId = "ch-1231";
        
        Action act = () => Gateway.Create(invalidName, apiPaymentId, apiChargeId);

        // Assert
        // ATUALIZADO: Troca de ArgumentException para InvalidPaymentParamsException
        act.Should().Throw<InvalidPaymentParamsException>()
            .WithMessage("Gateway name cannot be null or empty.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidApiPaymentId_ShouldThrowInvalidPaymentParamsException(string invalidApiPaymentId)
    {
        // Arrange
        const string name = "TestGateway";
        const string apiChargeId = "ch-1231";
        
        Action act = () => Gateway.Create(name, invalidApiPaymentId, apiChargeId);

        // Assert
        // ATUALIZADO: Troca de ArgumentException para InvalidPaymentParamsException
        act.Should().Throw<InvalidPaymentParamsException>()
            .WithMessage("API PaymentIntent ID cannot be null or empty."); 
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidApiChargeId_ShouldThrowInvalidPaymentParamsException(string invalidApiChargeId)
    {
        // Arrange
        const string name = "TestGateway";
        const string apiPaymentId = "pi-12345";
        
        Action act = () => Gateway.Create(name, apiPaymentId, invalidApiChargeId);

        // Assert
        // ATUALIZADO: Troca de ArgumentException para InvalidPaymentParamsException
        act.Should().Throw<InvalidPaymentParamsException>()
            .WithMessage("API Charge ID cannot be null or empty.");
    }
}