using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using FluentValidation;
using MassTransit;
using MongoDB.Driver;
using ProductService.API.Extensions;
using ProductService.Application.Commands.ProductsCommands.CreateProduct;
using ProductService.Application.Contracts;
using ProductService.Domain.Contracts;
using ProductService.Infrastructure.Consumers;
using ProductService.Infrastructure.MessageBus;
using ProductService.Infrastructure.Persistence;
using ProductService.Infrastructure.Persistence.Repositories;
using ProductService.Infrastructure.Services;

MongoDbPersistence.Configure();

var builder = WebApplication.CreateBuilder(args);

{
    builder.Services.AddSwagger();
    builder.Services.AddAuth(builder.Configuration);
}

#region MongoDB
var dbName = "productdb";
var connectionString = builder.Configuration["MONGODB_CONNECTION_STRING"];

if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("Environment Variable MONGODB_CONNECTION_STRING not found.");

builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));
builder.Services.AddScoped<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(dbName));
#endregion

// Mass Transit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ListingReadModelConsumer>();
    x.AddConsumer<DecrementWatchListConsumerService>();
    x.AddConsumer<IncrementWatchListConsumerService>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var connectionString = builder.Configuration["RABBITMQ_CONNECTION_STRING"];

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Environment Variable RABBITMQ_CONNECTION_STRING not found.");

        cfg.Host(new Uri(connectionString));

        cfg.ConfigureEndpoints(context);
    });
});
builder.Services.AddScoped<IMassTransitMessageBus, MassTransitMessageBus>();

builder.Services.AddScoped<IFileUploaderService, S3FileUploader>();

var awsAccessKey = builder.Configuration["AWS:AccessKey"];
var awsSecretKey = builder.Configuration["AWS:SecretKey"];
var awsRegion = builder.Configuration["AWS:Region"];
var awsBucketName = builder.Configuration["AWS:BucketName"];

if (string.IsNullOrEmpty(awsAccessKey) ||
    string.IsNullOrEmpty(awsSecretKey) ||
    string.IsNullOrEmpty(awsRegion) ||
    string.IsNullOrEmpty(awsBucketName))
{
    throw new InvalidOperationException("Missing required AWS settings. Check AWS:AccessKey, AWS:SecretKey, AWS:Region, and AWS:BucketName in configuration.");
}

var awsOptions = new AWSOptions
{
    Region = RegionEndpoint.GetBySystemName(awsRegion),
    Credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey)
};

builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonS3>(); builder.Services.AddScoped<IProductRepository, ProductRepository>();

var geminiApiKey = builder.Configuration["Gemini:Key"];

builder.Services.AddSingleton<IAiService>(new GeminiService(geminiApiKey ?? ""));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateProductCommandHandler).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(CreateProductCommandValidator).Assembly);

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles();
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

public partial class Program { }