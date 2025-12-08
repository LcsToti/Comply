using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using UserService.App.Interfaces;
using UserService.Domain.Interfaces;
using UserService.Infra.Persistence.Repositories;
using UserService.Infra.Services;

namespace UserService.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        #region MongoDB
        var connectionString = configuration["MONGODB_CONNECTION_STRING"];
        var dbName = "userdb";

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Environment Variable MONGODB_CONNECTION_STRING not found.");

        services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));
        services.AddScoped<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(dbName));
        #endregion

        #region RabbitMQ
        services.AddMassTransit(rabbitmq =>
        {
            rabbitmq.UsingRabbitMq((context, cfg) =>
            {
                var connectionString = configuration["RABBITMQ_CONNECTION_STRING"];

                if (string.IsNullOrEmpty(connectionString))
                    throw new InvalidOperationException("Environment Variable RABBITMQ_CONNECTION_STRING not found.");

                cfg.Host(new Uri(connectionString));

                cfg.ConfigureEndpoints(context);
            });
        });
        #endregion

        services.AddScoped<IUserRepository, MongoDbUserRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}