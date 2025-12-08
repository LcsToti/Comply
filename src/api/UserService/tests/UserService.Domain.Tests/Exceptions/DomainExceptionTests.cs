using FluentAssertions;
using UserService.Domain.Exceptions;

namespace UserService.Domain.Tests.Exceptions;

public class DomainExceptionTests
{
    [Fact]
    public void UserNotFoundException_WithEmail_ShouldSetMessage()
    {
        // Arrange
        var email = "user@example.com";

        // Act
        var exception = new UserNotFoundException(email);

        // Assert
        exception.Message.Should().Be($"Usuário com email '{email}' não encontrado.");
    }

    [Fact]
    public void UserNotFoundException_WithNullEmail_ShouldSetMessage()
    {
        // Arrange
        string? email = null;

        // Act
        var exception = new UserNotFoundException(email!);

        // Assert
        exception.Message.Should().Be("Usuário com email '' não encontrado.");
    }

    [Fact]
    public void UserAlreadyExistsException_WithEmail_ShouldSetMessage()
    {
        // Arrange
        var email = "user@example.com";

        // Act
        var exception = new UserAlreadyExistsException(email);

        // Assert
        exception.Message.Should().Be($"Usuário com email '{email}' já existe.");
    }

    [Fact]
    public void UserAlreadyExistsException_WithNullEmail_ShouldSetMessage()
    {
        // Arrange
        string? email = null;

        // Act
        var exception = new UserAlreadyExistsException(email!);

        // Assert
        exception.Message.Should().Be("Usuário com email '' já existe.");
    }

    [Fact]
    public void InvalidCredentialsException_ShouldSetMessage()
    {
        // Act
        var exception = new InvalidCredentialsException();

        // Assert
        exception.Message.Should().Be("Credenciais inválidas.");
    }

    [Fact]
    public void AllExceptions_ShouldInheritFromException()
    {
        // Act & Assert
        var userNotFoundException = new UserNotFoundException("test@example.com");
        var userAlreadyExistsException = new UserAlreadyExistsException("test@example.com");
        var invalidCredentialsException = new InvalidCredentialsException();

        userNotFoundException.Should().BeAssignableTo<Exception>();
        userAlreadyExistsException.Should().BeAssignableTo<Exception>();
        invalidCredentialsException.Should().BeAssignableTo<Exception>();
    }
}
