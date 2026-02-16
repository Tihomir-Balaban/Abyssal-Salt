using AbySalto.Mid.Application.Contracts;
using AbySalto.Mid.Infrastructure.ExternalProducts;
using AbySalto.Mid.Infrastructure.Persistence;
using AbySalto.Mid.Infrastructure.Repositories;
using AbySalto.Mid.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AbySalto.Mid.Application.Services;
using AbySalto.Mid.Infrastructure.ExternalProducts;
using Microsoft.Extensions.Caching.Memory;


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
        
        services.AddMemoryCache();
        
        // Clients
        services.AddHttpClient<DummyJsonProductClient>(client =>
        {
            client.BaseAddress = new Uri("https://dummyjson.com/");
        });

        services.AddScoped<IExternalProductClient>(sp =>
            new CachedExternalProductClient(
                sp.GetRequiredService<DummyJsonProductClient>(),
                sp.GetRequiredService<IMemoryCache>()));

        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IBasketRepository, BasketRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IFavoriteRepository, FavoriteRepository>();
        
        // Services
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenService, TokenService>();

        
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
