using FluentAssertions;
using UserService.Domain.Entities;

namespace UserService.Domain.Tests.Entities;

public class DeliveryAddressTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateDeliveryAddress()
    {
        // Arrange
        var street = "Rua das Flores";
        var number = "123";
        var city = "São Paulo";
        var state = "SP";
        var zipCode = "01234-567";

        // Act
        var address = new DeliveryAddress(street, number, city, state, zipCode);

        // Assert
        address.Street.Should().Be(street);
        address.Number.Should().Be(number);
        address.City.Should().Be(city);
        address.State.Should().Be(state);
        address.ZipCode.Should().Be(zipCode);
    }

    [Theory]
    [InlineData("", "123", "São Paulo", "SP", "01234-567", "street")]
    [InlineData("   ", "123", "São Paulo", "SP", "01234-567", "street")]
    [InlineData("Rua das Flores", "", "São Paulo", "SP", "01234-567", "number")]
    [InlineData("Rua das Flores", "   ", "São Paulo", "SP", "01234-567", "number")]
    [InlineData("Rua das Flores", "123", "", "SP", "01234-567", "city")]
    [InlineData("Rua das Flores", "123", "   ", "SP", "01234-567", "city")]
    [InlineData("Rua das Flores", "123", "São Paulo", "", "01234-567", "state")]
    [InlineData("Rua das Flores", "123", "São Paulo", "   ", "01234-567", "state")]
    [InlineData("Rua das Flores", "123", "São Paulo", "SP", "", "zipCode")]
    [InlineData("Rua das Flores", "123", "São Paulo", "SP", "   ", "zipCode")]
    public void Constructor_WithInvalidData_ShouldThrowArgumentException(
        string street, string number, string city, string state, string zipCode, string expectedParamName)
    {
        // Act & Assert
        var action = () => new DeliveryAddress(street, number, city, state, zipCode);
        action.Should().Throw<ArgumentException>()
            .WithMessage($"*{expectedParamName}*")
            .And.ParamName.Should().Be(expectedParamName);
    }

    [Fact]
    public void Constructor_WithWhitespaceData_ShouldTrimValues()
    {
        // Arrange
        var street = "  Rua das Flores  ";
        var number = "  123  ";
        var city = "  São Paulo  ";
        var state = "  SP  ";
        var zipCode = "  01234-567  ";

        // Act
        var address = new DeliveryAddress(street, number, city, state, zipCode);

        // Assert
        address.Street.Should().Be("Rua das Flores");
        address.Number.Should().Be("123");
        address.City.Should().Be("São Paulo");
        address.State.Should().Be("SP");
        address.ZipCode.Should().Be("01234-567");
    }

    [Fact]
    public void Equals_WithDifferentData_ShouldReturnFalse()
    {
        // Arrange
        var address1 = new DeliveryAddress("Rua A", "123", "São Paulo", "SP", "01234-567");
        var address2 = new DeliveryAddress("Rua B", "123", "São Paulo", "SP", "01234-567");

        // Act & Assert
        address1.Equals(address2).Should().BeFalse();
        (address1 == address2).Should().BeFalse();
        (address1 != address2).Should().BeTrue();
    }
}
