using FluentAssertions;
using Payments.Domain.Aggregates.PaymentAggregate.Factories;
using Payments.Domain.Exceptions.PaymentMethodExceptions;

namespace Payments.Domain.Tests.Aggregates.PaymentAggregate.Factories;

public class PaymentMethodFactoryTests
{
    [Fact]
    public void Create_WithValidParams_ShouldCreatePaymentMethod()
    {
        // Arrange
        const string id = "pm_123";
        const string type = "card";
        const string last4 = "1234";
        const string brand = "Visa";
        
        // Act
        var pm = PaymentMethodFactory.Create(id, type, last4, brand);
        
        // Assert
        pm.Should().NotBeNull();
        pm.Id.Should().Be(id);
        pm.Type.Should().Be(type);
        pm.Last4.Should().Be(last4);
        pm.Brand.Should().Be(brand);
    }
    
    [Fact]
    public void Create_WithNullOptionalParams_ShouldCreatePaymentMethod()
    {
        // Arrange
        const string id = "pm_pix_123";
        const string type = "pix";
        
        // Act
        var pm = PaymentMethodFactory.Create(id, type, null, null);
        
        // Assert
        pm.Should().NotBeNull();
        pm.Id.Should().Be(id);
        pm.Type.Should().Be(type);
        pm.Last4.Should().BeNull();
        pm.Brand.Should().BeNull();
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_WithInvalidId_ShouldThrowInvalidPaymentMethodParamsException(string invalidId)
    {
        // Act
        Action act = () => PaymentMethodFactory.Create(invalidId, "card", "1234", "Visa");
        
        // Assert
        act.Should().Throw<InvalidPaymentMethodParamsException>()
            .WithMessage("id");
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Create_WithInvalidType_ShouldThrowInvalidPaymentMethodParamsException(string invalidType)
    {
        // Act
        Action act = () => PaymentMethodFactory.Create("pm_123", invalidType, "1234", "Visa");
        
        // Assert
        act.Should().Throw<InvalidPaymentMethodParamsException>()
            .WithMessage("type");
    }
}