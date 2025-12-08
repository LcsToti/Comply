using FluentValidation;
using MediatR;
using SalesService.API.Middlewares;
using SalesService.App.Commands.SaleCommands.Dispute.AssignAdminToDispute;

namespace SalesService.API.Extensions;

public static class FluentValidationExtension
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<AssignAdminToDisputeCommandValidator>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        
        return services;
    }
}