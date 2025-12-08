using ProductsService.Domain.Tests.Commom;

namespace ProductsService.Domain.Tests;

public class FeatureProduct
{
    [Fact]
    public void AddFeatured_WhenCalledBySeller_ShouldAddFeatured()
    {
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        const int daysFeatured = 5;
        product.AddFeatured(sellerId, daysFeatured);

        Assert.True(product.Featured);
        Assert.Equal(DateTime.UtcNow.AddDays(daysFeatured).Date, product.ExpirationFeatureDate.Value.Date);
    }

    [Fact]
    public void AddFeatured_WhenCalledByAnotherUser_ShouldThrowInvalidOperationException()
    {
        var sellerId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        const int daysFeatured = 5;

        Assert.Throws<InvalidOperationException>(() => product.AddFeatured(anotherUserId, daysFeatured));
    }

    [Fact]
    public void AddFeatured_WhenAlreadyFeatured_ShouldThrowInvalidOperationException()
    {
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        const int daysFeatured = 5;
        product.AddFeatured(sellerId, daysFeatured);
        Assert.Throws<InvalidOperationException>(() => product.AddFeatured(sellerId, daysFeatured));
    }

    [Fact]
    public void AddFeatured_WhenDaysFeaturedIsZeroOrNegative_ShouldThrowArgumentOutOfRangeException()
    {
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        const int daysFeatured = 0;
        Assert.Throws<ArgumentOutOfRangeException>(() => product.AddFeatured(sellerId, daysFeatured));
    }

    [Fact]
    public void AddFeatured_WhenDaysFeaturedIsMoreThan30_ShouldThrowArgumentOutOfRangeException()
    {
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        const int daysFeatured = 31;
        Assert.Throws<ArgumentOutOfRangeException>(() => product.AddFeatured(sellerId, daysFeatured));
    }

    [Fact]
    public void ExtendFeatured_WhenCalledBySeller_ShouldExtendExpirationDate()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        product.AddFeatured(sellerId, 10); // Destaca por 10 dias
        var originalExpirationDate = product.ExpirationFeatureDate.Value;
        var daysToAdd = 5;

        // Act
        product.ExtendFeatured(sellerId, daysToAdd);

        // Assert
        Assert.Equal(originalExpirationDate.AddDays(daysToAdd), product.ExpirationFeatureDate.Value);
    }

    [Fact]
    public void ExtendFeatured_WhenProductIsNotFeatured_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => product.ExtendFeatured(sellerId, 5));
    }

    [Fact]
    public void ExtendFeatured_WhenTotalDaysExceedLimit_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        product.AddFeatured(sellerId, 25); // Já destacado por 25 dias

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => product.ExtendFeatured(sellerId, 10));
    }

    [Fact]
    public void ExtendFeatured_WhenCalledByDifferentUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        product.AddFeatured(sellerId, 15);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => product.ExtendFeatured(differentUserId, 5));
    }

    [Fact]
    public void ExtendFeatured_WhenDaysToAddIsZeroOrNegative_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        product.AddFeatured(sellerId, 10);
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => product.ExtendFeatured(sellerId, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => product.ExtendFeatured(sellerId, -5));
    }

    [Fact]
    public void CheckExpiration_WhenProductIsFeaturedAndExpired_ShouldUnfeaturedProduct()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);

        // Forçar um estado passado para o teste
        product.GetType().GetProperty("Featured")!.SetValue(product, true);
        product.GetType().GetProperty("ExpirationFeatureDate")?.SetValue(product, DateTime.UtcNow.AddDays(-1));

        // Act
        product.CheckExpiration();

        // Assert
        Assert.False(product.Featured);
        Assert.Null(product.ExpirationFeatureDate);
    }

    [Fact]
    public void CheckExpiration_WhenProductIsNotFeatured_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => product.CheckExpiration());
    }

    [Fact]
    public void CheckExpiration_WhenFeatureIsNotExpired_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        product.AddFeatured(sellerId, 7); // Destacado por 7 dias (não expirado)

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => product.CheckExpiration());
    }
}