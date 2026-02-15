using AbySalto.Mid.Application.Products;
using AbySalto.Mid.Application.Requests.Products;
using AbySalto.Mid.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AbySalto.Mid.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ProductQueries>();
        services.AddScoped<AuthService>();
        services.AddScoped<BasketService>();
        services.AddScoped<ProductImportService>();
        return services;
    }
}