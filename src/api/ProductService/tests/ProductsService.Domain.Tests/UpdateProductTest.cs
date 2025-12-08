using ProductService.Domain.Enums;
using ProductsService.Domain.Tests.Commom;

namespace ProductsService.Domain.Tests
{
    public class UpdateProductTests
    {
        [Fact]
        public void UpdateTitle_WhenCalledBySeller_ShouldUpdateTitle()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            var product = Common.CreateTestProduct(sellerId);
            var newTitle = "Novo Título Válido";

            // Act
            product.UpdateTitle(sellerId, newTitle);

            // Assert
            Assert.Equal(newTitle, product.Title);
        }

        [Fact]
        public void UpdateTitle_WhenCalledByDifferentUser_ShouldThrowArgumentException()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            var product = Common.CreateTestProduct(sellerId);
            var differentUserId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => product.UpdateTitle(differentUserId, "Título Inválido"));
        }


        [Fact]
        public void UpdateDescription_WhenCalledBySeller_ShouldUpdateDescription()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            var product = Common.CreateTestProduct(sellerId);
            var newDescription = "Nova Descrição Válida";

            // Act
            product.UpdateDescription(sellerId, newDescription);

            // Assert
            Assert.Equal(newDescription, product.Description);
        }

        [Fact]
        public void UpdateDescription_WhenCalledByDifferentUser_ShouldThrowArgumentException()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            var product = Common.CreateTestProduct(sellerId);
            var differentUserId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => product.UpdateDescription(differentUserId, "Descrição Inválida"));
        }


        [Fact]
        public void UpdateLocale_WhenCalledBySeller_ShouldUpdateLocale()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            var product = Common.CreateTestProduct(sellerId);
            var newLocale = "en-US";

            // Act
            product.UpdateLocale(sellerId, newLocale);

            // Assert
            Assert.Equal(newLocale, product.Locale);
        }

        [Fact]
        public void UpdateLocale_WhenCalledByDifferentUser_ShouldThrowArgumentException()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            var product = Common.CreateTestProduct(sellerId);
            var differentUserId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => product.UpdateLocale(differentUserId, "fr-FR"));
        }


        [Fact]
        public void UpdateCharacteristics_WhenCalledBySeller_ShouldUpdateCharacteristics()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            var product = Common.CreateTestProduct(sellerId);
            var newCharacteristics = new Dictionary<string, string> { { "Material", "Aço" } };

            // Act
            product.UpdateCharacteristics(sellerId, newCharacteristics);

            // Assert
            Assert.Equal(newCharacteristics, product.Characteristics);
        }

        [Fact]
        public void UpdateCharacteristics_WhenCalledByDifferentUser_ShouldThrowArgumentException()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            var product = Common.CreateTestProduct(sellerId);
            var differentUserId = Guid.NewGuid();
            var newCharacteristics = new Dictionary<string, string> { { "Material", "Aço" } };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => product.UpdateCharacteristics(differentUserId, newCharacteristics));
        }


        [Fact]
        public void UpdateCondition_WhenCalledBySeller_ShouldUpdateCondition()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            var product = Common.CreateTestProduct(sellerId);
            var newCondition = ProductCondition.Used;

            // Act
            product.UpdateCondition(sellerId, newCondition);

            // Assert
            Assert.Equal(newCondition, product.Condition);
        }

        [Fact]
        public void UpdateCondition_WhenCalledByDifferentUser_ShouldThrowArgumentException()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            var product = Common.CreateTestProduct(sellerId);
            var differentUserId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => product.UpdateCondition(differentUserId, ProductCondition.Used));
        }


        [Fact]
        public void UpdateCategory_WhenCalledBySeller_ShouldUpdateCategory()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            var product = Common.CreateTestProduct(sellerId);
            var newCategory = Categories.HomeAppliances;

            // Act
            product.UpdateCategory(sellerId, newCategory);

            // Assert
            Assert.Equal(newCategory, product.Category);
        }

        [Fact]
        public void UpdateCategory_WhenCalledByDifferentUser_ShouldThrowArgumentException()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            var product = Common.CreateTestProduct(sellerId);
            var differentUserId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => product.UpdateCategory(differentUserId, Categories.HomeAppliances));
        }


        [Fact]
        public void UpdateDeliveryPreference_WhenCalledBySeller_ShouldUpdatePreference()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            var product = Common.CreateTestProduct(sellerId);
            var newPreference = DeliveryPreferences.DeliveryService;

            // Act
            product.UpdateDeliveryPreference(sellerId, newPreference);

            // Assert
            Assert.Equal(newPreference, product.DeliveryPreference);
        }

        [Fact]
        public void UpdateDeliveryPreference_WhenCalledByDifferentUser_ShouldThrowArgumentException()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            var product = Common.CreateTestProduct(sellerId);
            var differentUserId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => product.UpdateDeliveryPreference(differentUserId, DeliveryPreferences.DeliveryService));
        }
    }
}