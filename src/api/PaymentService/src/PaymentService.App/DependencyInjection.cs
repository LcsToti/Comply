using Microsoft.Extensions.DependencyInjection;
using Payments.App.UseCases.PaymentCases.CreatePayment;

namespace Payments.App;

public static class DependencyInjection
{
    public static IServiceCollection AddApp(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreatePaymentEvent).Assembly));
        return services;
    }
}