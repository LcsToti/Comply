using ProductsService.Domain.Tests.Commom;

namespace ProductsService.Domain.Tests;

public class ImagesTests
{
    [Fact]
    public void AddImages_WithValidUrls_ShouldAddImagesToProduct()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        var imageUrls = new List<string> { "http://example.com/image1.jpg", "http://example.com/image2.jpg" };

        // Act
        product.AddImages(sellerId, imageUrls);

        // Assert
        Assert.Equal(2, product.Images.Count);
        Assert.Contains("http://example.com/image1.jpg", product.Images);
        Assert.Contains("http://example.com/image2.jpg", product.Images);
    }

    [Fact]
    public void RemoveImage_WithExistingUrl_ShouldRemoveImageFromProduct()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        var imageUrls = new List<string> { "http://example.com/image1.jpg", "http://example.com/image2.jpg" };
        product.AddImages(sellerId, imageUrls);

        var urlToRemove = new List<string> { "http://example.com/image1.jpg" };

        // Act
        product.RemoveImage(sellerId, urlToRemove);

        // Assert
        Assert.Single(product.Images);
        Assert.NotEqual(urlToRemove, product.Images);
    }


    [Fact]
    public void RemoveImage_WithNonExistentUrl_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        var urlToRemove = new List<string> { "http://example.com/nonexistent.jpg" };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => product.RemoveImage(sellerId, urlToRemove));
    }

    [Fact]
    public void ReorderImages_WithValidData_ShouldReorderSuccessfully()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var originalImages = new List<string> { "a.jpg", "b.jpg", "c.jpg" };
        var product = Common.CreateTestProduct(sellerId);
        product.AddImages(sellerId, originalImages);
        var newOrder = new List<string> { "c.jpg", "a.jpg", "b.jpg" };

        // Act
        product.ReorderImages(sellerId, newOrder);

        // Assert
        Assert.Equal(newOrder, product.Images);
        Assert.True(product.UpdatedAt > DateTime.MinValue);
    }

    [Fact]
    public void ReorderImages_WithDifferentUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var differentUser = Guid.NewGuid();
        var images = new List<string> { "a.jpg", "b.jpg", "c.jpg" };
        var product = Common.CreateTestProduct(sellerId);
        product.AddImages(sellerId, images);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            product.ReorderImages(differentUser, images));
    }

    [Fact]
    public void ReorderImages_WithNullList_ShouldThrowArgumentNullException()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var images = new List<string> { "1.jpg", "2.jpg" };
        var product = Common.CreateTestProduct(sellerId);
        product.AddImages(sellerId, images);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            product.ReorderImages(sellerId, new List<string>(null)));
    }

    [Fact]
    public void ReorderImages_WithDifferentCount_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var images = new List<string> { "a.jpg", "b.jpg", "c.jpg" };
        var product = Common.CreateTestProduct(sellerId);
        product.AddImages(sellerId, images);
        var invalidList = new List<string> { "a.jpg", "b.jpg" }; 

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            product.ReorderImages(sellerId, invalidList));
    }

    [Fact]
    public void ReorderImages_WithMissingImage_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var images = new List<string> { "a.jpg", "b.jpg", "c.jpg" };
        var product = Common.CreateTestProduct(sellerId);
        product.AddImages(sellerId, images);
        var invalidList = new List<string> { "a.jpg", "b.jpg", "x.jpg" };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            product.ReorderImages(sellerId, invalidList));
    }
}