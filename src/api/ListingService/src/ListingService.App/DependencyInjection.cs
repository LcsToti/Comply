using ListingService.App.Common;
using Microsoft.Extensions.DependencyInjection;

namespace ListingService.App;

public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationDependencyInjection).Assembly));
        services.AddScoped<RepositoryCommandsOrchestrator>();

        return services;
    }
}