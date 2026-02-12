using AbySalto.Mid.Application.Contracts;
using AbySalto.Mid.Infrastructure.ExternalProducts;
using AbySalto.Mid.Infrastructure.Persistence;
using AbySalto.Mid.Infrastructure.Repositories;
using AbySalto.Mid.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AbySalto.Mid.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                               ?? throw new InvalidOperationException("Connection string 'Default' not found.");

        if (connectionString == null) 
            throw new ArgumentNullException(nameof(connectionString));

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));
        
        services.AddScoped<IExternalProductClient, DummyJsonProductClient>();

        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IBasketRepository, BasketRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        // Services
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenService, TokenService>();

        // Clients
        services.AddHttpClient<IExternalProductClient, DummyJsonProductClient>(client =>
        {
            client.BaseAddress = new Uri("https://dummyjson.com/");
        });
        
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
