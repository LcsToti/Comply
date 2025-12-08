using FluentAssertions;
using MassTransit;
using Moq;
using UserService.App.Common.Results.Mappers;
using UserService.App.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Exceptions;
using UserService.Domain.Factories;
using UserService.Domain.Interfaces;
using UserService.Domain.ValueObjects;

namespace UserService.Domain.Tests.Services;

public class AuthenticationDomainServiceTests
{
    private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly App.Services.UserService _service;

    public AuthenticationDomainServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockPublishEndpoint = new Mock<IPublishEndpoint>();
        
        _service = new App.Services.UserService(_mockUserRepository.Object, _mockPasswordHasher.Object, _mockPublishEndpoint.Object);
    }

    [Fact]
    public async Task RegisterUserAsync_WithValidData_ShouldCreateAndReturnUser()
    {
        // Arrange
        var name = "John Doe";
        var email = new Email("john@example.com");
        var password = new Password("Password123!");
        var customerId = "customer_123";
        var connectedAccountId = "account_456";
        var hashedPassword = "hashed_password";

        _mockUserRepository.Setup(x => x.GetByEmailAsync(email.Value))
            .ReturnsAsync((User?)null);
        _mockPasswordHasher.Setup(x => x.HashPassword(password.Value))
            .Returns(hashedPassword);
        _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateUserAsync(name, email, password);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(name);
        result.Email.Should().Be(email.Value);
        result.PasswordHash.Should().Be(hashedPassword);

        _mockUserRepository.Verify(x => x.GetByEmailAsync(email.Value), Times.Once);
        _mockPasswordHasher.Verify(x => x.HashPassword(password.Value), Times.Once);
        _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterUserAsync_WithExistingUser_ShouldThrowUserAlreadyExistsException()
    {
        // Arrange
        var name = "John Doe";
        var email = new Email("john@example.com");
        var password = new Password("Password123!");
        var existingUser = UserFactory.Create("Jane Doe", "john@example.com", "existing_hash");

        _mockUserRepository.Setup(x => x.GetByEmailAsync(email.Value))
            .ReturnsAsync(existingUser);

        // Act & Assert
        var action = async () => await _service.CreateUserAsync(name, email, password);
        await action.Should().ThrowAsync<UserAlreadyExistsException>()
            .WithMessage($"Usuário com email '{email.Value}' já existe.");

        _mockUserRepository.Verify(x => x.GetByEmailAsync(email.Value), Times.Once);
        _mockPasswordHasher.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task RegisterUserAsync_WithInvalidName_ShouldThrowArgumentException(string? name)
    {
        // Arrange
        var email = new Email("john@example.com");
        var password = new Password("Password123!");

        // Act & Assert
        var action = async () => await _service.CreateUserAsync(name!, email, password);
        var exception = await action.Should().ThrowAsync<ArgumentException>();
        exception.Which.Message.Should().Contain("Nome é obrigatório");
        exception.Which.ParamName.Should().Be("name");
    }
    
    [Fact]
    public async Task AuthenticateUserAsync_WithNonExistentUser_ShouldThrowUserNotFoundException()
    {
        // Arrange
        var email = new Email("john@example.com");
        var password = new Password("Password123!");

        _mockUserRepository.Setup(x => x.GetByEmailAsync(email.Value))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var action = async () => await _service.GetUserByEmailAsync(email);
        await action.Should().ThrowAsync<UserNotFoundException>()
            .WithMessage($"Usuário com email '{email.Value}' não encontrado.");

        _mockUserRepository.Verify(x => x.GetByEmailAsync(email.Value), Times.Once);
        _mockPasswordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Constructor_WithNullUserRepository_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new App.Services.UserService(null!, _mockPasswordHasher.Object, _mockPublishEndpoint.Object);
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("userRepository");
    }

    [Fact]
    public void Constructor_WithNullPasswordHasher_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new App.Services.UserService(_mockUserRepository.Object, null!, _mockPublishEndpoint.Object);
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("passwordHasher");
    }
}
