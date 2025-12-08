using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.App.Interfaces;
using UserService.App.Services;
using UserService.App.Services.Auth;

namespace UserService.App;

public static class DependencyInjection
{
    public static IServiceCollection AddApp(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserService, Services.UserService>();
        
        return services;
    }
}