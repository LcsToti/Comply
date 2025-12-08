using FluentValidation;
using MediatR;
using Payments.API.Middlewares;
using Payments.App.UseCases.PaymentCases.CreatePayment;

namespace Payments.API.Extensions;

public static class FluentValidationExtension
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreatePaymentEventValidator>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        return services;
    }
}