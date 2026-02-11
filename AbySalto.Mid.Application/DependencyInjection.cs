using AbySalto.Mid.Application.Products;
using Microsoft.Extensions.DependencyInjection;

namespace AbySalto.Mid.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ProductQueries>();
        return services;
    }
}