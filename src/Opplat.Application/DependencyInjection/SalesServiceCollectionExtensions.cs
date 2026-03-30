using Microsoft.Extensions.DependencyInjection;
using Opplat.Application.Abstractions.Repositories.Sales;
using Opplat.Application.Features.Account.Commands;
using Opplat.Application.Services.Sales;
using Opplat.Infrastructure.Persistance.Repositories.Sales;
using Opplat.Infrastructure.Repositories.Sales;

namespace Opplat.Application.DependencyInjection;

public static class SalesServiceCollectionExtensions
{
    public static IServiceCollection AddSalesApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(ChangePasswordCommand).Assembly));
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
