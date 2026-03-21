using Opplat.Modules.Sales.Domain.Repositories;
using Opplat.Modules.Sales.Domain.Services;
using Opplat.Modules.Sales.Infrastructure.Repositories;

namespace Opplat.Services.Sales.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSalesModuleServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductRepository, ProductsRepository>();
        services.AddScoped<IToppingService, ToppingService>();
        services.AddScoped<IToppingRepository, ToppingRepository>();
        services.AddScoped<IProductTagService, ProductTagService>();
        services.AddScoped<IProductTagRepository, ProductTagRepository>();
        services.AddScoped<ICostTabService, CostTabService>();
        services.AddScoped<ICostTabRepository, CostTabRepository>();
        services.AddScoped<ISalesService, SalesService>();
        services.AddScoped<ISalesRepository, SalesRepository>();

        return services;
    }
}
