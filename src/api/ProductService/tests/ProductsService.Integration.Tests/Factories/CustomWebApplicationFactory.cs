using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using ProductsService.Integration.Tests.Fixture;
namespace ProductsService.Integration.Tests.Factories;

public class CustomWebApplicationFactory<TProgram>(ContainersFixture fixture) : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly ContainersFixture _fixture = fixture;
    public ContainersFixture ContainersFixture => _fixture;
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Debug);
        });
    }
}