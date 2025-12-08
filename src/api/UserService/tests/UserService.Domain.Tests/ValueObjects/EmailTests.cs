using FluentAssertions;
using UserService.Domain.ValueObjects;

namespace UserService.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test.email@domain.co.uk")]
    [InlineData("user+tag@example.org")]
    [InlineData("user123@test-domain.com")]
    public void Constructor_WithValidEmail_ShouldCreateEmail(string validEmail)
    {
        // Act
        var email = new Email(validEmail);

        // Assert
        email.Value.Should().Be(validEmail.Trim().ToLowerInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithNullOrEmptyEmail_ShouldThrowArgumentException(string? invalidEmail)
    {
        // Act & Assert
        var action = () => new Email(invalidEmail!);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Email não pode ser vazio ou nulo.*")
            .And.ParamName.Should().Be("value");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user.example.com")]
    [InlineData("user@.com")]
    [InlineData("user@example.")]
    [InlineData("user@@example.com")]
    public void Constructor_WithInvalidEmailFormat_ShouldThrowArgumentException(string invalidEmail)
    {
        // Act & Assert
        var action = () => new Email(invalidEmail);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Formato de email inválido.*")
            .And.ParamName.Should().Be("value");
    }
    
    [Fact]
    public void Constructor_WithUppercaseEmail_ShouldNormalizeToLowerCase()
    {
        // Arrange
        var uppercaseEmail = "USER@EXAMPLE.COM";

        // Act
        var email = new Email(uppercaseEmail);

        // Assert
        email.Value.Should().Be("user@example.com");
    }
    
    [Fact]
    public void Constructor_WithEmailContainingWhitespace_ShouldTrimAndNormalize()
    {
        // Arrange
        var emailWithWhitespace = "  user@example.com  ";

        // Act
        var email = new Email(emailWithWhitespace);

        // Assert
        email.Value.Should().Be("user@example.com");
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldReturnValue()
    {
        // Arrange
        var email = new Email("user@example.com");

        // Act
        string result = email;

        // Assert
        result.Should().Be("user@example.com");
    }

    [Fact]
    public void ImplicitConversion_FromString_ShouldCreateEmail()
    {
        // Act
        Email email = "user@example.com";

        // Assert
        email.Value.Should().Be("user@example.com");
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var email = new Email("user@example.com");

        // Act
        var result = email.ToString();

        // Assert
        result.Should().Be("user@example.com");
    }

    [Fact]
    public void Equals_WithSameEmail_ShouldReturnTrue()
    {
        // Arrange
        var email1 = new Email("user@example.com");
        var email2 = new Email("user@example.com");

        // Act & Assert
        email1.Equals(email2).Should().BeTrue();
        (email1 == email2).Should().BeTrue();
        (email1 != email2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentEmail_ShouldReturnFalse()
    {
        // Arrange
        var email1 = new Email("user1@example.com");
        var email2 = new Email("user2@example.com");

        // Act & Assert
        email1.Equals(email2).Should().BeFalse();
        (email1 == email2).Should().BeFalse();
        (email1 != email2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var email = new Email("user@example.com");

        // Act & Assert
        email.Equals(null).Should().BeFalse();
        (email == null!).Should().BeFalse();
        (email != null!).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_WithSameEmail_ShouldReturnSameHashCode()
    {
        // Arrange
        var email1 = new Email("user@example.com");
        var email2 = new Email("user@example.com");

        // Act
        var hashCode1 = email1.GetHashCode();
        var hashCode2 = email2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    public void GetHashCode_WithDifferentEmail_ShouldReturnDifferentHashCode()
    {
        // Arrange
        var email1 = new Email("user1@example.com");
        var email2 = new Email("user2@example.com");

        // Act
        var hashCode1 = email1.GetHashCode();
        var hashCode2 = email2.GetHashCode();

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }
}
