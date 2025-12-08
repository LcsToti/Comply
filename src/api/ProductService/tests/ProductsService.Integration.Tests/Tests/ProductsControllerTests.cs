using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ProductService.Application.Responses;
using ProductService.Domain.Entities;
using ProductService.Domain.Enums;
using ProductsService.Integration.Tests.Factories;
using ProductsService.Integration.Tests.Fixture;
using ProductsService.Integration.Tests.Seeders;
using ProductsService.Integration.Tests.Tests.Utils;

namespace ProductsService.Integration.Tests.Tests;

[Collection("Produtos - Testes de Integração")]
public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _httpClient;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly ContainersFixture _containersFixture;
    private readonly IMongoCollection<Product> _productCollection;
    private readonly IConfiguration _configuration;
    private readonly Guid _userId;

    public ProductsControllerTests(ContainersFixture containersFixture, CustomWebApplicationFactory<Program> factory)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Test.json", optional: false)
            .Build();

        Environment.SetEnvironmentVariable("MONGODB_CONNECTION_STRING", containersFixture.MongoConnection);
        Environment.SetEnvironmentVariable("RABBITMQ_CONNECTION_STRING", containersFixture.RabbitMqConnection);

        Environment.SetEnvironmentVariable("JwtSettings__Secret", config["JwtSettings:Secret"]);
        Environment.SetEnvironmentVariable("JwtSettings__Audience", config["JwtSettings:Audience"]);
        Environment.SetEnvironmentVariable("JwtSettings__Issuer", config["JwtSettings:Issuer"]);

        Environment.SetEnvironmentVariable("AWS__AccessKey", config["AWS:AccessKey"]);
        Environment.SetEnvironmentVariable("AWS__SecretKey", config["AWS:SecretKey"]);
        Environment.SetEnvironmentVariable("AWS__Region", config["AWS:Region"]);
        Environment.SetEnvironmentVariable("AWS__BucketName", config["AWS:BucketName"]);

        Environment.SetEnvironmentVariable("Gemini__Key", config["Gemini:Key"]);

        _containersFixture = containersFixture;
        _factory = factory;
        _configuration = _factory.Services.GetRequiredService<IConfiguration>();

        _userId = Guid.NewGuid();
        string jwtToken = FakeJwtGenerator.GenerateJwt(_configuration, "Admin", _userId);

        _httpClient = _factory.CreateClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var mongoClient = new MongoClient(_containersFixture.MongoConnection);
        var database = mongoClient.GetDatabase("productdb");
        _productCollection = database.GetCollection<Product>("Products");
        _productCollection.DeleteMany(FilterDefinition<Product>.Empty);
    }

    #region POST - Create
    [Fact(DisplayName = "POST /products: Deve criar um produto e persistir no MongoDB")]
    public async Task Post_CreateProduct_ShouldReturnCreated_AndPersistInMongo()
    {
        // ARRANGE
        var title = "GTX Titan 6GB FE - em otimas condicoes!";
        string description = "GTX TITAN 6GB – Usada, desempenho forte e ideal para quem quer potência com bom custo-benefício. "
            + "Placa topo de linha da época, ainda excelente para jogos, edição, modelagem 3D e qualquer aplicação que use CUDA. "
            + "Estado: totalmente funcional, nunca fez overclock, limpa, revisada, temperaturas estáveis, sem ruídos e sem reparos. "
            + "Especificações: 6GB GDDR5, 2688 CUDA Cores, barramento 384-bit, suporte a DirectX 12, OpenGL e CUDA. "
            + "Serve muito bem para Blender, Premiere, DaVinci Resolve, AutoCAD, 3D Max e projetos 3D médios. "
            + "Desempenho: roda jogos AAA em Full HD com boa taxa de FPS, excelente para eSports e ótima como GPU de edição. "
            + "Perfeita para quem quer performance real gastando pouco.";

        var img1 = FormFileLoader.Load("Tests/Assets/Images/GTX-Titan-1.png");
        var img2 = FormFileLoader.Load("Tests/Assets/Images/GTX-Titan-2.png");
        var locale = "Brazil";
        var fakeImages = new List<IFormFile> { img1, img2 };
        Dictionary<string, string> characteristics = new()
        {
            { "Modelo", "Founders Edition" },
            { "Memória", "6GB" },
            { "Tipo Memoria", "GDDR5" }
        };
        ProductCondition condition = ProductCondition.Used;
        Categories category = Categories.Electronics;
        DeliveryPreferences deliveryPreference = DeliveryPreferences.PickupPoint;

        var requestUri = "/api/v1/products";

        // Criar conteúdo multipart/form-data
        var content = new MultipartFormDataContent
        {
            { new StringContent(title), "Title" },
            { new StringContent(description), "Description" },
            { new StringContent(locale), "Locale" },
            { new StringContent(condition.ToString()), "Condition" },
            { new StringContent(category.ToString()), "Category" },
            { new StringContent(deliveryPreference.ToString()), "DeliveryPreference" }
        };

        // Adicionar características como JSON
        foreach (var kv in characteristics)
            content.Add(new StringContent(kv.Value), $"Characteristics[{kv.Key}]");
        
        // Adicionar arquivos
        foreach (var img in fakeImages)
            content.Add(new StreamContent(img.OpenReadStream()), "ImageUrls", img.FileName);

        // ACT
        var response = await _httpClient.PostAsync(requestUri, content);
        Console.WriteLine($"Status: {response.StatusCode}");
        var body = response.Content.ReadAsStringAsync();
        Console.WriteLine($"Body:\n{body}");

        // ASSERT
        // HTTP
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdProductResponse = await response.Content.ReadFromJsonAsync<ProductResponse>();
        createdProductResponse.Should().NotBeNull();
        var newProductId = createdProductResponse!.Id;

        // Persistencia
        await Task.Delay(500);

        var persistedProduct = await _productCollection
            .Find(p => p.ProductId == newProductId)
            .FirstOrDefaultAsync();

        persistedProduct.Should().NotBeNull("Produto deveria ter sido persistido no MongoDB");

        persistedProduct.SellerId.Should().Be(_userId);
        persistedProduct.Title.Should().Be(title);
        persistedProduct.Description.Should().Be(description);
        persistedProduct.Locale.Should().Be("Brazil");

        persistedProduct.Condition.Should().Be(condition);
        persistedProduct.Category.Should().Be(category);
        persistedProduct.DeliveryPreference.Should().Be(deliveryPreference);

        persistedProduct.Characteristics.Should().BeEquivalentTo(characteristics);

        persistedProduct.Images.Should().NotBeEmpty()
            .And.HaveCount(fakeImages.Count);

        persistedProduct.Images.All(url => url.Contains(".s3.amazonaws.com/")).Should().BeTrue();
    }

    [Fact(DisplayName = "POST /products: Não deve criar produto com conteúdo impróprio e não deve persistir no MongoDB")]
    public async Task Post_CreateProductWithInappropriateContent_ShouldReturnBadRequest_AndNotPersistInMongo()
    {
        // ARRANGE
        var title = "AK-47 Original, Quase Nova - Apenas para Colecionadores Sérios";
        string description = "Arma totalmente funcional, calibre 7.62mm, em ótimo estado de conservação, "
            + "nunca usada em combate. Inclui 3 carregadores. Venda apenas para quem tem porte e registro. "
            + "Ideal para defesa pessoal ou coleção de alto nível. Modelo clássico, dificil de encontrar.";

        // Assumindo que FormFileLoader.Load pode carregar imagens de 'armas'
        var img1 = FormFileLoader.Load("Tests/Assets/Images/Weapon-1.png"); // AK-47
        var img2 = FormFileLoader.Load("Tests/Assets/Images/Weapon-2.png"); // Detalhe do cano
        var img3 = FormFileLoader.Load("Tests/Assets/Images/Weapon-3.png"); // Cartuchos
        var locale = "Brazil";
        var fakeImages = new List<IFormFile> { img1, img2, img3 };
        Dictionary<string, string> characteristics = new()
        {
            { "Calibre", "7.62x39mm" },
            { "Tipo", "Rifle" }
        };
        ProductCondition condition = ProductCondition.Used;
        // Categoria adequada para algo ilegal/sensível
        Categories category = Categories.Others;
        DeliveryPreferences deliveryPreference = DeliveryPreferences.Both;

        var requestUri = "/api/v1/products";

        // Criar conteúdo multipart/form-data
        var content = new MultipartFormDataContent
        {
            { new StringContent(title), "Title" },
            { new StringContent(description), "Description" },
            { new StringContent(locale), "Locale" },
            { new StringContent(condition.ToString()), "Condition" },
            { new StringContent(category.ToString()), "Category" },
            { new StringContent(deliveryPreference.ToString()), "DeliveryPreference" }
        };

        // Adicionar características
        foreach (var kv in characteristics)
            content.Add(new StringContent(kv.Value), $"Characteristics[{kv.Key}]");

        // Adicionar arquivos
        foreach (var img in fakeImages)
            content.Add(new StreamContent(img.OpenReadStream()), "ImageUrls", img.FileName);

        // ACT
        var response = await _httpClient.PostAsync(requestUri, content);
        Console.WriteLine($"Status: {response.StatusCode}");
        var body = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Body:\n{body}");

        // ASSERT
        // HTTP
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError, "O conteúdo impróprio deveria ser rejeitado pela API");

        // Persistencia
        var persistedProduct = await _productCollection
            .Find(p => p.Title == title || p.Description == description)
            .FirstOrDefaultAsync();

        persistedProduct.Should().BeNull("Produto com conteúdo impróprio NÃO deveria ter sido persistido no MongoDB");
    }
    #endregion

    #region PATCH - Update
    [Fact(DisplayName = "PATCH /products/{productId}: Deve atualizar um produto existente e persistir no MongoDB")]
    public async Task Patch_UpdateProduct_ShouldReturnOk_AndPersistUpdateInMongo()
    {
        // ARRANGE
        var existingProduct = await ProductSeeder.SeedInappropriateProduct(_productCollection, _userId);
        var productIdToUpdate = existingProduct.ProductId;

        var newTitle = "GTX TITAN 6GB - Versão ATUALIZADA e REVISADA!";
        var newDescription = "Descrição revisada para a placa topo de linha. Performance aprimorada e estética renovada.";

        var locale = "Minas Gerais";

        Dictionary<string, string> updatedCharacteristics = new()
        {
            { "Modelo", "Founders Edition" },
            { "Memória", "6GB GDDR5" },
            { "Interface", "384-bit" }
        };

        ProductCondition newCondition = ProductCondition.Refurbished;
        Categories newCategory = Categories.Electronics;
        DeliveryPreferences newDeliveryPreference = DeliveryPreferences.DeliveryService;

        var requestUri = $"/api/v1/products/{productIdToUpdate}";

        var content = new MultipartFormDataContent
        {
            { new StringContent(newTitle), "Title" },
            { new StringContent(newDescription), "Description" },
            { new StringContent(newCondition.ToString()), "Condition" },
            { new StringContent(locale), "Locale" },
            { new StringContent(newCategory.ToString()), "Category" },
            { new StringContent(newDeliveryPreference.ToString()), "DeliveryPreference" }
        };

        foreach (var kv in updatedCharacteristics)
            content.Add(new StringContent(kv.Value), $"Characteristics[{kv.Key}]");

        // ACT
        var response = await _httpClient.PatchAsync(requestUri, content);
        Console.WriteLine($"Status: {response.StatusCode}");
        var body = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Body:\n{body}");

        // ASSERT
        // HTTP
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedProductResponse = await response.Content.ReadFromJsonAsync<ProductResponse>();
        updatedProductResponse.Should().NotBeNull();
        updatedProductResponse!.Id.Should().Be(productIdToUpdate);

        // Persistencia
        await Task.Delay(500);

        var persistedProduct = await _productCollection
            .Find(p => p.ProductId == productIdToUpdate)
            .FirstOrDefaultAsync();

        persistedProduct.Should().NotBeNull("O produto atualizado deveria ter sido persistido no MongoDB");

        persistedProduct.Title.Should().Be(newTitle);
        persistedProduct.Description.Should().Be(newDescription);

        persistedProduct.Characteristics.Should().BeEquivalentTo(updatedCharacteristics);
    }

    [Fact(DisplayName = "PATCH /products/{productId}: Deve retornar 500 ao tentar atualizar com item proibido (AK47)")]
    public async Task Patch_UpdateProduct_ShouldReturn500_WhenForbiddenWeapon()
    {
        // ARRANGE
        var existingProduct = await ProductSeeder.SeedInappropriateProduct(_productCollection, _userId);
        var productIdToUpdate = existingProduct.ProductId;

        var requestUri = $"/api/v1/products/{productIdToUpdate}";

        var content = new MultipartFormDataContent
        {
            { new StringContent("AK47 Real - Alta Letalidade"), "Title" },
            { new StringContent("Arma de fogo real, calibre 7.62, extremamente perigosa."), "Description" },
            { new StringContent("New"), "Condition" },
            { new StringContent("São Paulo"), "Locale" },
            { new StringContent("Electronics"), "Category" },
            { new StringContent("DeliveryService"), "DeliveryPreference" },
        }; 

        Dictionary<string, string> updatedCharacteristics = new()
        {
            { "Calibre", "7.62x39mm" },
            { "Carregador", "30 tiros" },
            { "TipoDisparo", "Automática" }
        };

        foreach (var kv in updatedCharacteristics)
            content.Add(new StringContent(kv.Value), $"Characteristics[{kv.Key}]");

        // ACT
        var response = await _httpClient.PatchAsync(requestUri, content);
        var body = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"Status: {response.StatusCode}");
        Console.WriteLine($"Body:\n{body}");

        // ASSERT
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var persistedProduct = await _productCollection
            .Find(p => p.ProductId == productIdToUpdate)
            .FirstOrDefaultAsync();

        persistedProduct.Title.Should().Be(existingProduct.Title);
        persistedProduct.Description.Should().Be(existingProduct.Description);
        persistedProduct.Characteristics.Should().BeEquivalentTo(existingProduct.Characteristics);
    }
    #endregion
}