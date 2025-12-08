using System.ComponentModel.DataAnnotations;
using ProductService.Domain.Enums;
using ProductService.Domain.Factories;

namespace ProductsService.Domain.Tests
{
    public class CreateProductTests
    {
        [Fact]
        public void Create_WithValidParameters_ShouldCreateProductSuccessfully()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            const string title = "Produto de Teste Válido";
            const string description = "Uma descrição detalhada.";
            const string locale = "RJ";
            var characteristics = new Dictionary<string, string> { { "Cor", "Azul" } };
            const ProductCondition condition = ProductCondition.New;
            const Categories category = Categories.Electronics;
            const DeliveryPreferences deliveryPreference = DeliveryPreferences.Both;

            // Act
            var product = ProductFactory.Create(
                sellerId,
                title,
                description,
                locale,
                characteristics,
                condition,
                category,
                deliveryPreference
            );

            Assert.NotNull(product);
            Assert.Equal(title, product.Title);
            Assert.Equal(description, product.Description);
            Assert.Equal(locale, product.Locale);
            Assert.Equal(characteristics, product.Characteristics);
            Assert.Equal(condition, product.Condition);
            Assert.Equal(category, product.Category);
            Assert.Equal(deliveryPreference, product.DeliveryPreference);
            Assert.Equal(sellerId, product.SellerId);
            Assert.NotEqual(Guid.Empty, product.ProductId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithNullOrEmptyTitle_ShouldThrowArgumentException(string invalidTitle)
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            var title = invalidTitle;
            const string description = "Uma descrição.";
            const string locale = "RJ";
            var characteristics = new Dictionary<string, string> { { "Cor", "Azul" } };
            const ProductCondition condition = ProductCondition.New;
            const Categories category = Categories.Electronics;
            const DeliveryPreferences deliveryPreference = DeliveryPreferences.Both;

            Assert.Throws<ArgumentException>(() =>
                ProductFactory.Create(
                    sellerId,
                    title,
                    description,
                    locale,
                    characteristics,
                    condition,
                    category,
                    deliveryPreference
                )
            );
        }

        [Fact]
        public void Create_WithEmptySellerId_ShouldThrowArgumentException()
        {
            // Arrange
            var sellerId = Guid.Empty;
            const string title = "Produto de Teste";
            const string description = "Uma descrição.";
            const string locale = "RJ";
            var characteristics = new Dictionary<string, string> { { "Cor", "Azul" } };
            const ProductCondition condition = ProductCondition.New;
            const Categories category = Categories.Electronics;
            const DeliveryPreferences deliveryPreference = DeliveryPreferences.Both;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                ProductFactory.Create(
                    sellerId,
                    title,
                    description,
                    locale,
                    characteristics,
                    condition,
                    category,
                    deliveryPreference
                )
            );
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void Create_WithNullOrEmptyDescription_ShouldThrowArgumentNullException(string invalidDescription)
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            const string title = "Produto de Teste";
            const string locale = "RJ";
            var description = invalidDescription;
            var characteristics = new Dictionary<string, string> { { "Cor", "Azul" } };
            const ProductCondition condition = ProductCondition.New;
            const Categories category = Categories.Electronics;
            const DeliveryPreferences deliveryPreference = DeliveryPreferences.Both;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ProductFactory.Create(
                    sellerId,
                    title,
                    description,
                    locale,
                    characteristics,
                    condition,
                    category,
                    deliveryPreference
                )
            );
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void Create_WithNullOrEmptyLocale_ShouldThrowArgumentNullException(string invalidLocale)
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            const string title = "Produto de Teste";
            const string description = "Descrição do produto";
            var characteristics = new Dictionary<string, string> { { "Cor", "Azul" } };
            const ProductCondition condition = ProductCondition.New;
            const Categories category = Categories.Electronics;
            const DeliveryPreferences deliveryPreference = DeliveryPreferences.Both;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ProductFactory.Create(
                    sellerId,
                    title,
                    description,
                    invalidLocale,
                    characteristics,
                    condition,
                    category,
                    deliveryPreference
                )
            );
        }

        [Fact]
        public void Create_WithInvalidCharacteristicsCount_ShouldThrowArgumentException()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            const string title = "Produto de Teste";
            const string description = "Descrição do produto";
            const string locale = "RJ";
            var characteristics = Enumerable.Range(1, 51).ToDictionary(i => $"Key{i}", i => $"Value{i}");

            const ProductCondition condition = ProductCondition.New;
            const Categories category = Categories.Electronics;
            const DeliveryPreferences deliveryPreference = DeliveryPreferences.Both;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                ProductFactory.Create(
                    sellerId,
                    title,
                    description,
                    locale,
                    characteristics,
                    condition,
                    category,
                    deliveryPreference
                )
            );
        }


        [Fact]
        public void Create_WithMaxCharacteristics_ShouldCreateSuccessfully()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            const string title = "Produto Teste";
            const string description = "Descrição válida";
            const string locale = "SP";
            var characteristics = new Dictionary<string, string>();
            for (var i = 0; i < 10; i++)
            {
                characteristics.Add($"Key{i}", $"Value{i}");
            }

            const ProductCondition condition = ProductCondition.New;
            const Categories category = Categories.Electronics;
            const DeliveryPreferences deliveryPreference = DeliveryPreferences.Both;

            // Act
            var product = ProductFactory.Create(
                sellerId,
                title,
                description,
                locale,
                characteristics,
                condition,
                category,
                deliveryPreference
            );

            // Assert
            Assert.NotNull(product);
            Assert.Equal(10, product.Characteristics.Count);
        }

        [Theory]
        [InlineData((ProductCondition)999)]
        public void Create_WithInvalidProductCondition_ShouldThrowArgumentException(ProductCondition invalidCondition)
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            const string title = "Produto Teste";
            const string description = "Descrição válida";
            const string locale = "SP";
            var condition = invalidCondition;
            var characteristics = new Dictionary<string, string> { { "Cor", "Azul" } };
            const Categories category = Categories.Electronics;
            const DeliveryPreferences deliveryPreference = DeliveryPreferences.Both;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                ProductFactory.Create(
                    sellerId,
                    title,
                    description,
                    locale,
                    characteristics,
                    condition,
                    category,
                    deliveryPreference
                )
            );
        }

        [Theory]
        [InlineData((Categories)999)]
        public void Create_WithInvalidProductCategory_ShouldThrowArgumentException(Categories invalidCondition)
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            const string title = "Produto Teste";
            const string description = "Descrição válida";
            const string locale = "SP";
            const ProductCondition condition = ProductCondition.New;
            var characteristics = new Dictionary<string, string> { { "Cor", "Azul" } };
            var category = invalidCondition;
            const DeliveryPreferences deliveryPreference = DeliveryPreferences.Both;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                ProductFactory.Create(
                    sellerId,
                    title,
                    description,
                    locale,
                    characteristics,
                    condition,
                    category,
                    deliveryPreference
                )
            );
        }

        [Fact]
        public void Create_TwoProducts_ShouldGenerateDifferentIds()
        {
            // Arrange
            var sellerId = Guid.NewGuid();
            const string title = "Produto Teste";
            const string description = "Descrição válida";
            const string locale = "SP";
            var characteristics = new Dictionary<string, string> { { "Cor", "Azul" } };
            const ProductCondition condition = ProductCondition.New;
            const Categories category = Categories.Electronics;
            const DeliveryPreferences deliveryPreference = DeliveryPreferences.Both;

            // Act
            var product1 = ProductFactory.Create(sellerId, title, description, locale, characteristics, condition,
                category, deliveryPreference);
            var product2 = ProductFactory.Create(sellerId, title, description, locale, characteristics, condition,
                category, deliveryPreference);

            // Assert
            Assert.NotEqual(product1.ProductId, product2.ProductId);
        }

        [Fact]
        public void Create_WithQna_ShouldCreateQnaObject()
        {

        }
    }
}