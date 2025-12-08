using FluentAssertions;
using Moq;
using UserService.App.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Enums;
using UserService.Domain.Factories;
using UserService.Domain.ValueObjects;

namespace UserService.Domain.Tests.Entities;

public class UserTests
{
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;

    public UserTests()
    {
        _mockPasswordHasher = new Mock<IPasswordHasher>();
    }

    [Fact]
    public void Constructor_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var name = "John Doe";
        var email = "john@example.com";
        var passwordHash = "hashed_password";

        // Act
        var user = UserFactory.Create(name, email, passwordHash);

        // Assert
        user.Name.Should().Be(name);
        user.Email.Should().Be(email.ToLowerInvariant());
        user.PasswordHash.Should().Be(passwordHash);
        user.Role.Should().Be(Role.User);
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        user.PhoneNumber.Should().BeNull();
        user.ProfilePic.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithEmailValueObject_ShouldCreateUser()
    {
        // Arrange
        var name = "John Doe";
        var email = new Email("john@example.com");
        var passwordHash = "hashed_password";

        // Act
        var user = UserFactory.Create(name, email, passwordHash);

        // Assert
        user.Name.Should().Be(name);
        user.Email.Should().Be(email.Value);
        user.PasswordHash.Should().Be(passwordHash);
        user.Role.Should().Be(Role.User);
    }

    [Theory]
    [InlineData("", "john@example.com", "hashed_password", "name")]
    [InlineData("   ", "john@example.com", "hashed_password", "name")]
    [InlineData("John Doe", "", "hashed_password", "email")]
    [InlineData("John Doe", "   ", "hashed_password", "email")]
    [InlineData("John Doe", "john@example.com", "", "passwordHash")]
    [InlineData("John Doe", "john@example.com", "   ", "passwordHash")]
    public void Constructor_WithInvalidData_ShouldThrowArgumentException(
        string name, string email, string passwordHash, string expectedParamName)
    {
        // Act & Assert
        var action = () => UserFactory.Create(name, email, passwordHash);
        action.Should().Throw<ArgumentException>()
            .WithMessage($"*{expectedParamName}*")
            .And.ParamName.Should().Be(expectedParamName);
    }
    
    [Fact]
    public void UpdateProfile_WithValidData_ShouldUpdateProperties()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");
        var newName = "Jane Doe";
        var phoneNumber = "1234567890";
        var profilePic = "profile.jpg";

        // Act
        user.ChangeName(newName);
        user.ChangePhoneNumber(phoneNumber);
        user.UpdateProfilePicture(profilePic);

        // Assert
        user.Name.Should().Be(newName);
        user.PhoneNumber.Should().Be(phoneNumber);
        user.ProfilePic.Should().Be(profilePic);
    }

    [Fact]
    public void UpdateProfile_WithNullValues_ShouldNotUpdateProperties()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");
        var originalName = user.Name;

        // Act
        user.ChangeName(null);
        user.ChangePhoneNumber(null);
        user.UpdateProfilePicture(null);

        // Assert
        user.Name.Should().Be(originalName);
        user.PhoneNumber.Should().BeNull();
        user.ProfilePic.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateProfile_WithNullOrWhitespaceValues_ShouldNotUpdateProperties(string? invalidValue)
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");
        var originalName = user.Name;

        // Act
        user.ChangeName(invalidValue);
        user.ChangePhoneNumber(invalidValue);
        user.UpdateProfilePicture(invalidValue);

        // Assert
        user.Name.Should().Be(originalName); // O nome não deve mudar
        user.PhoneNumber.Should().BeNull(); // Assumindo que o valor inicial é null
        user.ProfilePic.Should().BeNull(); // Assumindo que o valor inicial é null
    }
    
    [Fact]
    public void RemoveFromWatchList_WithNonExistentProductId_ShouldNotThrowException()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");
        var existingProductId = "product_123";
        var nonExistentProductId = "product_456";
        user.AddToWatchList(existingProductId);

        // Act
        Action action = () => user.RemoveFromWatchList(nonExistentProductId);

        // Assert
        action.Should().NotThrow();
        user.WatchList.Should().HaveCount(1).And.Contain(existingProductId);
    }
    
    [Fact]
    public void AddDeliveryAddress_WithValidAddress_ShouldAddToCollection()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");
        var address = new DeliveryAddress("Rua A", "123", "São Paulo", "SP", "01234-567");

        // Act
        user.AddDeliveryAddress(address);

        // Assert
        user.DeliveryAddresses.Should().Contain(address);
        user.DeliveryAddresses.Should().HaveCount(1);
    }

    [Fact]
    public void AddDeliveryAddress_WithNullAddress_ShouldThrowArgumentNullException()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");

        // Act & Assert
        var action = () => user.AddDeliveryAddress(null!);
        action.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("address");
    }

    [Fact]
    public void AddDeliveryAddress_WithDuplicateAddress_ShouldNotAddAgain()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");
        var address = new DeliveryAddress("Rua A", "123", "São Paulo", "SP", "01234-567");
        user.AddDeliveryAddress(address);

        // Act
        user.AddDeliveryAddress(address);

        // Assert
        user.DeliveryAddresses.Should().HaveCount(1);
    }

    [Fact]
    public void UpdateDeliveryAddress_WithValidAddresses_ShouldUpdateAddress()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");
        var existingAddress = new DeliveryAddress("Rua A", "123", "São Paulo", "SP", "01234-567");
        var updatedAddress = new DeliveryAddress("Rua B", "456", "Rio de Janeiro", "RJ", "20000-000");
        user.AddDeliveryAddress(existingAddress);

        // Act
        user.UpdateDeliveryAddress(existingAddress, updatedAddress);

        // Assert
        user.DeliveryAddresses.Should().Contain(updatedAddress);
        user.DeliveryAddresses.Should().NotContain(existingAddress);
    }

    [Fact]
    public void UpdateDeliveryAddress_WithNonExistentAddress_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");
        var existingAddress = new DeliveryAddress("Rua A", "123", "São Paulo", "SP", "01234-567");
        var updatedAddress = new DeliveryAddress("Rua B", "456", "Rio de Janeiro", "RJ", "20000-000");

        // Act & Assert
        var action = () => user.UpdateDeliveryAddress(existingAddress, updatedAddress);
        
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Endereço não encontrado.");
    }

    [Fact]
    public void RemoveDeliveryAddress_WithValidAddress_ShouldRemoveFromCollection()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");
        var address = new DeliveryAddress("Rua A", "123", "São Paulo", "SP", "01234-567");
        user.AddDeliveryAddress(address);

        // Act
        user.RemoveDeliveryAddress(address);

        // Assert
        user.DeliveryAddresses.Should().NotContain(address);
        user.DeliveryAddresses.Should().BeEmpty();
    }

    [Fact]
    public void RemoveDeliveryAddress_WithNonExistentAddress_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");
        var address = new DeliveryAddress("Rua A", "123", "São Paulo", "SP", "01234-567");

        // Act & Assert
        var action = () => user.RemoveDeliveryAddress(address);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Endereço não encontrado.");
    }

    [Fact]
    public void ChangeRole_ShouldUpdateRole()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");

        // Act
        user.ChangeRole(Role.Admin);

        // Assert
        user.Role.Should().Be(Role.Admin);
    }

    [Theory]
    [InlineData(Role.User)]
    [InlineData(Role.Admin)]
    [InlineData(Role.Moderator)]
    public void HasRole_WithMatchingRole_ShouldReturnTrue(Role role)
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");
        user.ChangeRole(role);

        // Act
        var result = user.HasRole(role);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasRole_WithNonMatchingRole_ShouldReturnFalse()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");
        user.ChangeRole(Role.User);

        // Act
        var result = user.HasRole(Role.Admin);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsAdmin_WithAdminRole_ShouldReturnTrue()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");
        user.ChangeRole(Role.Admin);

        // Act
        var result = user.IsAdmin();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsAdmin_WithNonAdminRole_ShouldReturnFalse()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");

        // Act
        var result = user.IsAdmin();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsModerator_WithModeratorRole_ShouldReturnTrue()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");
        user.ChangeRole(Role.Moderator);

        // Act
        var result = user.IsModerator();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsModerator_WithNonModeratorRole_ShouldReturnFalse()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");

        // Act
        var result = user.IsModerator();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsUser_WithUserRole_ShouldReturnTrue()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");

        // Act
        var result = user.IsUser();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsUser_WithNonUserRole_ShouldReturnFalse()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");
        user.ChangeRole(Role.Admin);

        // Act
        var result = user.IsUser();

        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void AddToWatchList_WithValidProductId_ShouldAddToCollection()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");
        var productId = "product_123";

        // Act
        user.AddToWatchList(productId);

        // Assert
        user.WatchList.Should().Contain(productId);
        user.WatchList.Should().HaveCount(1);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void AddToWatchList_WithInvalidProductId_ShouldThrowArgumentException(string? productId)
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");

        // Act & Assert
        var action = () => user.AddToWatchList(productId!);
        action.Should().Throw<ArgumentException>()
            .WithMessage("ID do produto inválido.*")
            .And.ParamName.Should().Be("productId");
    }

    [Fact]
    public void AddToWatchList_WithDuplicateProductId_ShouldNotAddAgain()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");
        var productId = "product_123";
        user.AddToWatchList(productId);

        // Act
        user.AddToWatchList(productId);

        // Assert
        user.WatchList.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveFromWatchList_WithValidProductId_ShouldRemoveFromCollection()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");
        var productId = "product_123";
        user.AddToWatchList(productId);

        // Act
        user.RemoveFromWatchList(productId);

        // Assert
        user.WatchList.Should().NotContain(productId);
        user.WatchList.Should().BeEmpty();
    }

    [Fact]
    public void VerifyPassword_WithValidPassword_ShouldReturnTrue()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");
        var password = "password123";
        _mockPasswordHasher.Setup(x => x.VerifyPassword(password, user.PasswordHash))
            .Returns(true);

        // Act
        var result = _mockPasswordHasher.Object.VerifyPassword(password, user.PasswordHash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithInvalidPassword_ShouldReturnFalse()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");
        var password = "wrong_password";
        _mockPasswordHasher.Setup(x => x.VerifyPassword(password, user.PasswordHash))
            .Returns(false);

        // Act
        var result = _mockPasswordHasher.Object.VerifyPassword(password, user.PasswordHash);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void VerifyPassword_WithNullOrEmptyPassword_ShouldReturnFalse(string? password)
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "hashed_password");

        // Act
        var result = _mockPasswordHasher.Object.VerifyPassword(password, user.PasswordHash);

        // Assert
        result.Should().BeFalse();
        _mockPasswordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void ChangePassword_WithValidHash_ShouldUpdatePasswordHash()
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "old_hash");
        var newHash = "new_hash";

        // Act
        user.ChangePassword(newHash);

        // Assert
        user.PasswordHash.Should().Be(newHash);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ChangePassword_WithInvalidHash_ShouldThrowArgumentException(string? newHash)
    {
        // Arrange
        var user = UserFactory.Create("John Doe", "john@example.com", "old_hash");

        // Act & Assert
        var action = () => user.ChangePassword(newHash!);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Hash da senha não pode ser vazio.*")
            .And.ParamName.Should().Be("newPasswordHash");
    }
}
