using FluentAssertions;
using UserService.Domain.ValueObjects;

namespace UserService.Domain.Tests.ValueObjects;

public class PasswordTests
{
    [Theory]
    [InlineData("Password123!")]
    [InlineData("MySecure1@")]
    [InlineData("Test123$")]
    [InlineData("ValidPass1%")]
    public void Constructor_WithValidPassword_ShouldCreatePassword(string validPassword)
    {
        // Act
        var password = new Password(validPassword);

        // Assert
        password.Value.Should().Be(validPassword);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithNullOrEmptyPassword_ShouldThrowArgumentException(string? invalidPassword)
    {
        // Act & Assert
        var action = () => new Password(invalidPassword!);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Senha não pode ser vazia ou nula.*")
            .And.ParamName.Should().Be("value");
    }

    [Theory]
    [InlineData("short")]
    [InlineData("1234567")]
    [InlineData("abcdefg")]
    public void Constructor_WithPasswordShorterThan8Characters_ShouldThrowArgumentException(string shortPassword)
    {
        // Act & Assert
        var action = () => new Password(shortPassword);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Senha deve ter pelo menos 8 caracteres.*")
            .And.ParamName.Should().Be("value");
    }

    [Theory]
    [InlineData("password123")] // No uppercase, no special char
    [InlineData("PASSWORD123")] // No lowercase, no special char
    [InlineData("Password")] // No digit, no special char
    [InlineData("Password123")] // No special char
    [InlineData("Password!")] // No digit
    [InlineData("password!")] // No uppercase, no digit
    [InlineData("PASSWORD!")] // No lowercase, no digit
    public void Constructor_WithInvalidPasswordFormat_ShouldThrowArgumentException(string invalidPassword)
    {
        // Act & Assert
        var action = () => new Password(invalidPassword);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Senha deve conter pelo menos: 1 letra minúscula, 1 letra maiúscula, 1 dígito e 1 caractere especial (@$!%*?&).*")
            .And.ParamName.Should().Be("value");
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldReturnValue()
    {
        // Arrange
        var password = new Password("Password123!");

        // Act
        string result = password;

        // Assert
        result.Should().Be("Password123!");
    }

    [Fact]
    public void ImplicitConversion_FromString_ShouldCreatePassword()
    {
        // Act
        Password password = "Password123!";

        // Assert
        password.Value.Should().Be("Password123!");
    }

    [Fact]
    public void ToString_ShouldReturnMaskedValue()
    {
        // Arrange
        var password = new Password("Password123!");

        // Act
        var result = password.ToString();

        // Assert
        result.Should().Be("***");
    }

    [Fact]
    public void Equals_WithSamePassword_ShouldReturnTrue()
    {
        // Arrange
        var password1 = new Password("Password123!");
        var password2 = new Password("Password123!");

        // Act & Assert
        password1.Equals(password2).Should().BeTrue();
        (password1 == password2).Should().BeTrue();
        (password1 != password2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentPassword_ShouldReturnFalse()
    {
        // Arrange
        var password1 = new Password("Password123!");
        var password2 = new Password("Password456!");

        // Act & Assert
        password1.Equals(password2).Should().BeFalse();
        (password1 == password2).Should().BeFalse();
        (password1 != password2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var password = new Password("Password123!");

        // Act & Assert
        password.Equals(null).Should().BeFalse();
        (password == null!).Should().BeFalse();
        (password != null!).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_WithSamePassword_ShouldReturnSameHashCode()
    {
        // Arrange
        var password1 = new Password("Password123!");
        var password2 = new Password("Password123!");

        // Act
        var hashCode1 = password1.GetHashCode();
        var hashCode2 = password2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    public void GetHashCode_WithDifferentPassword_ShouldReturnDifferentHashCode()
    {
        // Arrange
        var password1 = new Password("Password123!");
        var password2 = new Password("Password456!");

        // Act
        var hashCode1 = password1.GetHashCode();
        var hashCode2 = password2.GetHashCode();

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }
}
